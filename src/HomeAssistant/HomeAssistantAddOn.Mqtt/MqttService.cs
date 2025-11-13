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
using System.Security.Authentication;

namespace HomeAssistantAddOn.Mqtt;

/// <summary>
/// MQTT service for connecting to Home Assistant MQTT broker.
/// Handles connection management, message publishing, and topic subscriptions with automatic reconnection.
/// </summary>
public class MqttService : IDisposable
{
    private readonly ILogger<MqttService> _logger;
    private readonly IManagedMqttClient _mqttClient;
    private readonly SupervisorApi _supervisorApi;
    private readonly IOptionsMonitor<MqttOptions> _optionsMonitor;
    private readonly ConcurrentDictionary<string, List<Func<string, Task>>> _subscriptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MqttService"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="optionsMonitor">MQTT configuration options monitor.</param>
    /// <param name="supervisorApi">Home Assistant Supervisor API for auto-configuration.</param>
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
        _mqttClient.ConnectingFailedAsync += ConnectingFailedAsync;
        _mqttClient.ApplicationMessageSkippedAsync += ApplicationMessageSkippedAsync;
        _mqttClient.DisconnectedAsync += DisconnectedAsync;
        _mqttClient.ConnectedAsync += ConnectedAsync;
    }

    /// <summary>
    /// Handles MQTT client connected event.
    /// </summary>
    /// <param name="args">Connection event arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task ConnectedAsync(MqttClientConnectedEventArgs args)
    {
        _logger.LogTrace("Connected {ConnectionResult}", JsonConvert.SerializeObject(args.ConnectResult));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles MQTT client disconnected event.
    /// </summary>
    /// <param name="args">Disconnection event arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task DisconnectedAsync(MqttClientDisconnectedEventArgs args)
    {
        _logger.LogTrace(args.Exception, "Disconnected {ConnectionResult},{Reason}", JsonConvert.SerializeObject(args.ConnectResult), args.ReasonString);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles application message skipped event when message cannot be processed.
    /// </summary>
    /// <param name="args">Message skipped event arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task ApplicationMessageSkippedAsync(ApplicationMessageSkippedEventArgs args)
    {
        _logger.LogError("ApplicationMessageSkipped {Topic}", args.ApplicationMessage.ApplicationMessage.Topic);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles MQTT connection failure event.
    /// </summary>
    /// <param name="args">Connection failure event arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task ConnectingFailedAsync(ConnectingFailedEventArgs args)
    {
        _logger.LogError(args.Exception, "ConnectingFailed {ConnectionResult}", JsonConvert.SerializeObject(args.ConnectResult));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Starts the MQTT client and connects to the broker.
    /// Supports auto-configuration from Home Assistant Supervisor or manual configuration.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
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
                        builder.WithTlsOptions(opt =>{
                            opt.WithSslProtocols(SslProtocols.Tls12);
                        });
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
                        builder.WithTlsOptions(opt => {
                            opt.WithSslProtocols(SslProtocols.Tls12);
                        });
                    }
                }
            })
            .Build();
        await _mqttClient.StartAsync(_mqttOption);
        _logger.LogDebug("Start");
    }

    /// <summary>
    /// Stops the MQTT client, unsubscribes from all topics, and disconnects from the broker.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Subscribes to an MQTT topic and registers a handler function for received messages.
    /// Multiple handlers can be registered for the same topic.
    /// </summary>
    /// <param name="topic">MQTT topic to subscribe to.</param>
    /// <param name="subscribeTask">Async function to handle received messages.</param>
    public void Subscribe(string topic, Func<string, Task> subscribeTask)
    {
        _subscriptions.AddOrUpdate(topic
            , _ =>
            {
                _mqttClient.SubscribeAsync(topic);
                return [subscribeTask];
            }
            , (_, list) =>
            {
                list.Add(subscribeTask);
                return list;
            });
        _logger.LogDebug("Subscribe {topic}", topic);
    }

    /// <summary>
    /// Handles received MQTT application messages and invokes registered subscription handlers.
    /// </summary>
    /// <param name="args">Message received event arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        if (_subscriptions.TryGetValue(args.ApplicationMessage.Topic, out var subscribeTasks))
        {
            var payload = args.ApplicationMessage.ConvertPayloadToString();
            await Task.WhenAll(subscribeTasks.Select(subscribeTask => subscribeTask.Invoke(payload)));
        }
        _logger.LogDebug("Receive {topic}", args.ApplicationMessage.Topic);
    }

    /// <summary>
    /// JSON serializer settings for MQTT payloads, excluding null values.
    /// </summary>
    private static readonly JsonSerializerSettings MqttPaloadSerializerSetting = new() { NullValueHandling = NullValueHandling.Ignore };

    /// <summary>
    /// Publishes an object as JSON to an MQTT topic.
    /// </summary>
    /// <param name="topic">MQTT topic to publish to.</param>
    /// <param name="payloadObject">Object to serialize and publish.</param>
    /// <param name="retain">Whether to retain the message on the broker.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PublishAsync(string topic, object payloadObject, bool retain = false)
    {
        var payload = JsonConvert.SerializeObject(payloadObject, MqttPaloadSerializerSetting);
        await _mqttClient.EnqueueAsync(new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(retain)
            .Build());
        _logger.LogDebug("Publish {topic} {payload}", topic, payload);
    }

    /// <summary>
    /// Publishes a string payload to an MQTT topic.
    /// </summary>
    /// <param name="topic">MQTT topic to publish to.</param>
    /// <param name="payload">String payload to publish.</param>
    /// <param name="retain">Whether to retain the message on the broker.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PublishAsync(string topic, string payload, bool retain = false)
    {
        await _mqttClient.EnqueueAsync(new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(retain)
            .Build());
        _logger.LogDebug("Publish {topic} {payload}", topic, payload);
    }

    /// <summary>
    /// Disposes the MQTT client and releases resources.
    /// </summary>
    public void Dispose()
    {
        _mqttClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}