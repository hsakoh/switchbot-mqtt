using Microsoft.Extensions.Options;
using SwitchBotMqttApp.Configurations;

namespace SwitchBotMqttApp.Services;

public class AutomatedHostedService : IHostedService
{
    private readonly ILogger<AutomatedHostedService> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly MqttCoreService _deviceManagerService;
    private readonly PollingService _pollingService;
    private readonly WebhookService _webhookService;
    private readonly IOptions<CommonOptions> _commonOptions;

    public AutomatedHostedService(
        ILogger<AutomatedHostedService> logger,
        IHostApplicationLifetime lifetime,
        MqttCoreService deviceManagerService,
        PollingService pollingService,
        WebhookService webhookService,
        IOptions<CommonOptions> commonOptions)
    {
        _logger = logger;
        _lifetime = lifetime;
        _deviceManagerService = deviceManagerService;
        _pollingService = pollingService;
        _webhookService = webhookService;
        _commonOptions = commonOptions;
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_commonOptions.Value.AutoStartServices)
        {
            var combinedCancellationToken = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken, _lifetime.ApplicationStopping)
                .Token;

            _ = _lifetime.ApplicationStarted.Register(() =>
            {
                _logger.LogDebug("Application has started - will start services");
                _ = _deviceManagerService.StartAsync(combinedCancellationToken).ContinueWith((_) =>
                {
                    _ = _pollingService.StartAsync(combinedCancellationToken);
                });
                _ = _webhookService.StartAsync(combinedCancellationToken);
            });
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Application has stopped - will stop services");
        return Task.WhenAll(
            _deviceManagerService.StopAsync(cancellationToken),
            _pollingService.StopAsync(cancellationToken),
            _webhookService.StopAsync(cancellationToken)
        );
    }
}
