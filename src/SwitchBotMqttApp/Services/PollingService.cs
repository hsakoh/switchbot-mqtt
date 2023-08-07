namespace SwitchBotMqttApp.Services;

public class PollingService : ManagedServiceBase
{
    private readonly ILogger<PollingService> _logger;
    private readonly MqttCoreService _mqttCoreService;
    public PollingService(
        ILogger<PollingService> logger
        , MqttCoreService mqttCoreService)
    {
        _logger = logger;
        _mqttCoreService = mqttCoreService;
    }

    private readonly List<System.Timers.Timer> pollingTimers = new();

    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Status = ServiceStatus.Starting;
            if (_mqttCoreService.Status != ServiceStatus.Started)
            {
                _logger.LogWarning("{Service} has not started", nameof(MqttCoreService));
                Status = ServiceStatus.Failed;
                return Task.CompletedTask;
            }
            var pollingDevice = _mqttCoreService.CurrentDevicesConfig.PhysicalDevices.Where(d => d.UsePolling).ToList();
            pollingDevice.ForEach(d =>
            {
                var timer = new System.Timers.Timer
                {
                    Interval = d.PollingInterval.TotalMilliseconds
                };
                timer.Elapsed += async (_, _) =>
                {
                    await _mqttCoreService.PollingAndPublishStatusAsync(d, cancellationToken);
                };
                timer.Start();
                pollingTimers.Add(timer);
            });

            _logger.LogInformation("started");
            Status = ServiceStatus.Started;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(StartAsync)}  failed.");
            Status = ServiceStatus.Failed;
        }
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken = default)
    {
        foreach (var pollingTimer in pollingTimers)
        {
            pollingTimer.Stop();
        }
        pollingTimers.Clear();
        _logger.LogInformation("stopped");
        Status = ServiceStatus.Stoped;
        return Task.CompletedTask;
    }
}
