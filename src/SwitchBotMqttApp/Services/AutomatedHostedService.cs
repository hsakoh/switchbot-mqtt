using Microsoft.Extensions.Options;
using SwitchBotMqttApp.Configurations;

namespace SwitchBotMqttApp.Services;

/// <summary>
/// Automated hosted service that manages the lifecycle of core application services.
/// Coordinates startup sequence: MQTT Core -> Polling Service and Webhook Service in parallel.
/// </summary>
public class AutomatedHostedService(
    ILogger<AutomatedHostedService> logger,
    IHostApplicationLifetime lifetime,
    MqttCoreService deviceManagerService,
    PollingService pollingService,
    WebhookService webhookService,
    IOptions<CommonOptions> commonOptions) : IHostedService
{
    /// <summary>
    /// Starts the automated service lifecycle management.
    /// Initiates MQTT Core Service, followed by Polling and Webhook services when AutoStartServices is enabled.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (commonOptions.Value.AutoStartServices)
        {
            var combinedCancellationToken = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken, lifetime.ApplicationStopping)
                .Token;

            _ = lifetime.ApplicationStarted.Register(() =>
            {
                logger.LogDebug("Application has started - will start services");
                _ = deviceManagerService.StartAsync(combinedCancellationToken).ContinueWith((_) =>
                {
                    _ = pollingService.StartAsync(combinedCancellationToken);
                });
                _ = webhookService.StartAsync(combinedCancellationToken);
            });
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops all managed services in parallel when the application is shutting down.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Application has stopped - will stop services");
        return Task.WhenAll(
            deviceManagerService.StopAsync(cancellationToken),
            pollingService.StopAsync(cancellationToken),
            webhookService.StopAsync(cancellationToken)
        );
    }
}
