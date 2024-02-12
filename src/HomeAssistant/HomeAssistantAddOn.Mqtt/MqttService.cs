using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Reflection;

namespace HomeAssistantAddOn.Mqtt;
public class MqttService : IDisposable
{
    private readonly ILogger<MqttService> _logger;
    private readonly IManagedMqttClient _mqttClient;
    private readonly SupervisorApi _supervisorApi;
    private readonly IOptionsMonitor<MqttOptions> _optionsMonitor;
    private readonly ConcurrentDictionary<string, List<Func<string, Task>>> _subscriptions = new();

    public MqttService(
        ILogger<MqttService> logger,
        IOptionsMonitor<MqttOptions> optionsMonitor,
        SupervisorApi supervisorApi)
    {
        _logger = logger;
        _optionsMonitor = optionsMonitor;
        _supervisorApi = supervisorApi;

        var mqttFactory = new MqttFactory();
        _mqttClient = mqttFactory.CreateManagedMqttClient();
        _mqttClient.ApplicationMessageReceivedAsync += ApplicationMessageReceivedAsync;
    }

    public async Task StartAsync()
    {
        if (_mqttClient.IsStarted)
        {
            _logger.LogDebug("Already Started");
            return;
        }
        var options = _optionsMonitor.CurrentValue;
        SupervisorApi.ServiceMqtt? serviceMqtt = null;
        if (options.AutoConfig)
        {
            var response = await _supervisorApi.GetServicesMqtt();
            if (response?.Result != "ok")
            {
                throw new InvalidOperationException($"mqtt autoconfig failed {response?.Result}");
            }
            serviceMqtt = response.Data;
        }
        var _mqttOption = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(10))
            .WithClientOptions((builder) =>
            {
                var options = _optionsMonitor.CurrentValue;
                if (options.AutoConfig)
                {
                    builder
                        .WithTcpServer(serviceMqtt!.Host, serviceMqtt.Port)
                        .WithClientId(Assembly.GetEntryAssembly()!.FullName);
                    if (!string.IsNullOrEmpty(serviceMqtt.Username))
                    {
                        builder.WithCredentials(serviceMqtt.Username, string.IsNullOrEmpty(serviceMqtt.Password) ? null : serviceMqtt.Password);
                    }
                    if (serviceMqtt.Ssl)
                    {
                        builder.WithTls();
                    }
                }
                else
                {
                    builder
                        .WithTcpServer(options.Host, options.Port)
                        .WithClientId(Assembly.GetEntryAssembly()!.FullName);
                    if (!string.IsNullOrEmpty(options.Id))
                    {
                        builder.WithCredentials(options.Id, string.IsNullOrEmpty(options.Pw) ? null : options.Pw);
                    }
                    if (options.Tls)
                    {
                        builder.WithTls();
                    }
                }
            })
            .Build();
        await _mqttClient.StartAsync(_mqttOption);
        _logger.LogDebug("Start");
    }

    public async Task StopAsync()
    {
        foreach(var subscription in _subscriptions)
        {
            await _mqttClient.UnsubscribeAsync(subscription.Key);
        }
        _subscriptions.Clear();
        await _mqttClient.StopAsync();
        _logger.LogDebug("Stop");
    }

    public void Subscribe(string topic, Func<string, Task> subscribeTask)
    {
        _subscriptions.AddOrUpdate(topic
            , _ =>
            {
                _mqttClient.SubscribeAsync(topic);
                return new List<Func<string, Task>> { subscribeTask };
            }
            , (_, list) =>
            {
                list.Add(subscribeTask);
                return list;
            });
        _logger.LogDebug("Subscribe {topic}", topic);
    }

    private async Task ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        if (_subscriptions.TryGetValue(args.ApplicationMessage.Topic, out var subscribeTasks))
        {
            var payload = args.ApplicationMessage.ConvertPayloadToString();
            await Task.WhenAll(subscribeTasks.Select(subscribeTask => subscribeTask.Invoke(payload)));
        }
        _logger.LogDebug("Receive {topic}", args.ApplicationMessage.Topic);
    }

    private static readonly JsonSerializerSettings MqttPaloadSerializerSetting = new() { NullValueHandling = NullValueHandling.Ignore };

    public async Task PublishAsync(string topic, object payload, bool retain = false)
    {
        await _mqttClient.EnqueueAsync(new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(JsonConvert.SerializeObject(payload, MqttPaloadSerializerSetting))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(retain)
            .Build());
        _logger.LogDebug("Publish {topic}", topic);
    }

    public async Task PublishAsync(string topic, string jsonPayload, bool retain = false)
    {
        await _mqttClient.EnqueueAsync(new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(jsonPayload)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(retain)
            .Build());
        _logger.LogDebug("Publish {topic}", topic);
    }

    public void Dispose()
    {
        _mqttClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}