using FluffySpoon.Ngrok;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;
using SwitchBotMqttApp.Configurations;
using SwitchBotMqttApp.Logics;

namespace SwitchBotMqttApp.Services;

public class WebhookService : ManagedServiceBase
{
    private readonly ILogger<WebhookService> _logger;
    private readonly SwitchBotApiClient _switchBotApiClient;
    private readonly IOptions<WebhookServiceOptions> _webhookServiceOptions;
    private readonly INgrokService _ngrokService;
    private readonly IServer _server;
    private string? webhookUrl = null;
    public WebhookService(
        ILogger<WebhookService> logger
        , IOptions<WebhookServiceOptions> webhookServiceOptions
        , INgrokService ngrokService
        , SwitchBotApiClient switchBotApiClient
        , IServer server)
    {
        _logger = logger;
        _switchBotApiClient = switchBotApiClient;
        _webhookServiceOptions = webhookServiceOptions;
        _ngrokService = ngrokService;
        _server = server;
        if (!_webhookServiceOptions.Value.UseWebhook)
        {
            Status = ServiceStatus.Disabled;
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (!_webhookServiceOptions.Value.UseWebhook)
        {
            Status = ServiceStatus.Disabled;
            return;
        }
        try
        {
            Status = ServiceStatus.Starting;
            if (_webhookServiceOptions.Value.UseNgrok)
            {
                await _ngrokService.InitializeAsync(cancellationToken);

                IServerAddressesFeature? feature = _server.Features.Get<IServerAddressesFeature>()
                    ?? throw new InvalidOperationException("Ngrok requires the IServerAddressesFeature to be accessible.");
                var address = feature.Addresses
                    .Select(x => new Uri(x))
                    .OrderBy(x => x.Port)
                    .First();

                var tunnel = await _ngrokService.StartAsync(address, cancellationToken);
                webhookUrl = tunnel.PublicUrl;
            }
            else
            {
                webhookUrl = _webhookServiceOptions.Value.HostUrl;
            }
            webhookUrl += "/webhook";
            var webhooks = await _switchBotApiClient.GetWebhooksAsync(cancellationToken);

            if (webhooks.Urls.Contains(webhookUrl))
            {
                var details = await _switchBotApiClient.GetWebhookAsync([webhookUrl], cancellationToken);
                if (!details.First().Enable)
                {
                    await _switchBotApiClient.UpdateWebhookAsync(webhookUrl, enable: true, cancellationToken);
                }
            }
            else
            {
                await _switchBotApiClient.ConfigureWebhook(webhookUrl, cancellationToken);
            }
            _logger.LogInformation("start listen {webhookUrl}", webhookUrl);
            Status = ServiceStatus.Started;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(StartAsync)}  failed.");
            Status = ServiceStatus.Failed;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_webhookServiceOptions.Value.UseNgrok)
            {
                await _ngrokService.StopAsync(cancellationToken);

                typeof(NgrokService).GetField("_isInitialized", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(_ngrokService, false);
            }
            if (webhookUrl != null)
            {
                await _switchBotApiClient.UpdateWebhookAsync(webhookUrl, enable: false, cancellationToken);
                await _switchBotApiClient.DeleteWebhookAsync(webhookUrl, cancellationToken);
                webhookUrl = null;
            }
            _logger.LogInformation("stopped");
            Status = ServiceStatus.Stoped;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(StopAsync));
            Status = ServiceStatus.Failed;
            throw;
        }
    }
}
