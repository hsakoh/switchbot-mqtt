using Microsoft.Extensions.Options;
using SwitchBotMqttApp.Configurations;

namespace SwitchBotMqttApp.Services;

public class AutomatedHostedService(
    ILogger<AutomatedHostedService> logger,
    IHostApplicationLifetime lifetime,
    MqttCoreService deviceManagerService,
    PollingService pollingService,
    WebhookService webhookService,
    IOptions<CommonOptions> commonOptions) : IHostedService
{
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
