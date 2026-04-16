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
/// Includes an automatic watchdog that restarts the service if it fails.
/// </summary>
public class WebhookService : ManagedServiceBase
{
    private readonly ILogger<WebhookService> _logger;
    private readonly SwitchBotApiClient _switchBotApiClient;
    private readonly IOptions<WebhookServiceOptions> _webhookServiceOptions;
    private readonly INgrokService _ngrokService;
    private readonly IServer _server;
    private string? webhookUrl = null;

    private CancellationTokenSource? _watchdogCts;
    private Task? _watchdogTask;

    // How long to wait between watchdog checks (60 seconds)
    private static readonly TimeSpan WatchdogInterval = TimeSpan.FromSeconds(60);
    // How long to wait before retrying after a failure (30 seconds)
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(30);

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
    /// Also starts a background watchdog that automatically restarts the service on failure.
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

        await StartInternalAsync(cancellationToken);

        // Start the watchdog background loop
        _watchdogCts = new CancellationTokenSource();
        _watchdogTask = Task.Run(() => WatchdogLoopAsync(_watchdogCts.Token), CancellationToken.None);
    }

    /// <summary>
    /// Internal start logic — can be called both on initial start and by the watchdog on restart.
    /// </summary>
    private async Task StartInternalAsync(CancellationToken cancellationToken = default)
    {
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
            _logger.LogError(ex, $"{nameof(StartInternalAsync)} failed.");
            Status = ServiceStatus.Failed;
        }
    }

    /// <summary>
    /// Background watchdog loop. Checks every 60 seconds if the service is still running.
    /// If the status is Failed, it attempts a clean restart after a short delay.
    /// </summary>
    private async Task WatchdogLoopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Webhook watchdog started.");
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(WatchdogInterval, cancellationToken);

                if (Status == ServiceStatus.Failed)
                {
                    _logger.LogWarning("Webhook watchdog detected Failed status. Attempting restart in {delay}s...", RetryDelay.TotalSeconds);
                    await Task.Delay(RetryDelay, cancellationToken);

                    // Clean up Ngrok state before restarting
                    if (_webhookServiceOptions.Value.UseNgrok)
                    {
                        try
                        {
                            await _ngrokService.StopAsync(cancellationToken);
                            typeof(NgrokService).GetField("_isInitialized", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                                .SetValue(_ngrokService, false);
                        }
                        catch (Exception cleanupEx)
                        {
                            _logger.LogWarning(cleanupEx, "Webhook watchdog: cleanup before restart failed (ignored).");
                        }
                    }
                    webhookUrl = null;

                    _logger.LogInformation("Webhook watchdog: restarting WebhookService now.");
                    await StartInternalAsync(cancellationToken);

                    if (Status == ServiceStatus.Started)
                    {
                        _logger.LogInformation("Webhook watchdog: restart successful.");
                    }
                    else
                    {
                        _logger.LogWarning("Webhook watchdog: restart failed again, will retry in next cycle.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook watchdog encountered an unexpected error.");
            }
        }
        _logger.LogInformation("Webhook watchdog stopped.");
    }

    /// <summary>
    /// Stops the webhook service, disables and deletes the webhook from SwitchBot Cloud,
    /// stops Ngrok tunnel if used, and stops the watchdog.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task StopAsync(CancellationToken cancellationToken = default)
    {
        // Stop the watchdog first so it doesn't interfere with shutdown
        if (_watchdogCts != null)
        {
            _watchdogCts.Cancel();
            if (_watchdogTask != null)
            {
                try { await _watchdogTask; } catch { /* ignored */ }
            }
        }

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
