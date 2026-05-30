using CloudflaredKit;
using FluffySpoon.Ngrok;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SwitchBotMqttApp.Configurations;
using SwitchBotMqttApp.Logics;

namespace SwitchBotMqttApp.Services;

/// <summary>
/// Manages webhook registration with SwitchBot Cloud for real-time device event notifications.
/// Supports multiple tunnel modes: Disabled, HostUrl, Ngrok, TryCloudflare, and CloudflareZeroTrust.
/// </summary>
public class WebhookService : ManagedServiceBase
{
    private readonly ILogger<WebhookService> _logger;
    private readonly SwitchBotApiClient _switchBotApiClient;
    private readonly IOptions<WebhookServiceOptions> _webhookServiceOptions;
    private readonly IOptions<CommonOptions> _commonOptions;
    private readonly INgrokService _ngrokService;
    private readonly ICloudflaredService _cloudflaredService;
    private readonly IServer _server;
    private readonly IHostApplicationLifetime _appLifetime;
    private string? webhookUrl = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebhookService"/> class.
    /// </summary>
    public WebhookService(
        ILogger<WebhookService> logger
        , IOptions<WebhookServiceOptions> webhookServiceOptions
        , IOptions<CommonOptions> commonOptions
        , INgrokService ngrokService
        , ICloudflaredService cloudflaredService
        , SwitchBotApiClient switchBotApiClient
        , IServer server
        , IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _switchBotApiClient = switchBotApiClient;
        _webhookServiceOptions = webhookServiceOptions;
        _commonOptions = commonOptions;
        _ngrokService = ngrokService;
        _cloudflaredService = cloudflaredService;
        _server = server;
        _appLifetime = appLifetime;
        if (_webhookServiceOptions.Value.TunnelMode == WebhookTunnelMode.Disabled)
        {
            Status = ServiceStatus.Disabled;
        }
    }

    /// <summary>
    /// Starts the webhook service, establishes the tunnel (if configured), and registers webhook with SwitchBot Cloud.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_webhookServiceOptions.Value.TunnelMode == WebhookTunnelMode.Disabled)
        {
            Status = ServiceStatus.Disabled;
            return;
        }
        try
        {
            Status = ServiceStatus.Starting;
            var mode = _webhookServiceOptions.Value.TunnelMode;

            if (mode == WebhookTunnelMode.Ngrok)
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
            else if (mode == WebhookTunnelMode.TryCloudflare)
            {
                var tunnel = await _cloudflaredService.StartAsync(cancellationToken);
                webhookUrl = tunnel.PublicUrl;
            }
            else if (mode == WebhookTunnelMode.CloudflareZeroTrust)
            {
                await _cloudflaredService.StartAsync(cancellationToken);
                // In permanent tunnel mode, PublicUrl is null. Use the configured HostUrl.
                webhookUrl = _webhookServiceOptions.Value.HostUrl;
            }
            else // HostUrl
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
            _logger.LogError(ex, $"{nameof(StartAsync)} failed.");
            Status = ServiceStatus.Failed;
            if (_commonOptions.Value.ExitOnServiceFailure)
            {
                _logger.LogCritical("ExitOnServiceFailure is enabled. Stopping application.");
                _appLifetime.StopApplication();
            }
        }
    }

    /// <summary>
    /// Stops the webhook service, disables and deletes the webhook from SwitchBot Cloud,
    /// and stops the tunnel if used.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task StopAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var mode = _webhookServiceOptions.Value.TunnelMode;

            if (mode == WebhookTunnelMode.Ngrok)
            {
                await _ngrokService.StopAsync(cancellationToken);
                typeof(NgrokService).GetField("_isInitialized", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(_ngrokService, false);
            }
            else if (mode == WebhookTunnelMode.TryCloudflare || mode == WebhookTunnelMode.CloudflareZeroTrust)
            {
                await _cloudflaredService.StopAsync(cancellationToken);
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
