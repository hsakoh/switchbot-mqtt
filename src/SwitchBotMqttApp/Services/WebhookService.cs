using FluffySpoon.Ngrok;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;
using SwitchBotMqttApp.Configurations;
using SwitchBotMqttApp.Logics;

namespace SwitchBotMqttApp.Services;

/// <summary>
/// Manages webhook registration with SwitchBot Cloud for real-time device event notifications.
/// Supports both direct webhook URL and Ngrok tunnel for development/testing.
/// </summary>
public class WebhookService : ManagedServiceBase
{
    private readonly ILogger<WebhookService> _logger;
    private readonly SwitchBotApiClient _switchBotApiClient;
    private readonly IOptions<WebhookServiceOptions> _webhookServiceOptions;
    private readonly INgrokService _ngrokService;
    private readonly IServer _server;
    private string? webhookUrl = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebhookService"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="webhookServiceOptions">Webhook service configuration options.</param>
    /// <param name="ngrokService">Ngrok service for creating tunnels.</param>
    /// <param name="switchBotApiClient">SwitchBot API client.</param>
    /// <param name="server">ASP.NET Core server instance.</param>
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

    /// <summary>
    /// Starts the webhook service, creates Ngrok tunnel if configured, and registers webhook with SwitchBot Cloud.
    /// Enables existing webhooks if already registered.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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

                await Task.Delay(5000, cancellationToken);

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

    /// <summary>
    /// Stops the webhook service, disables and deletes the webhook from SwitchBot Cloud,
    /// and stops Ngrok tunnel if used.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
