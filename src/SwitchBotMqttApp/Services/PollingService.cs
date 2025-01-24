namespace SwitchBotMqttApp.Services;

public class PollingService(
    ILogger<PollingService> logger
        , MqttCoreService mqttCoreService) : ManagedServiceBase
{
    private readonly List<System.Timers.Timer> pollingTimers = [];

    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Status = ServiceStatus.Starting;
            if (mqttCoreService.Status != ServiceStatus.Started)
            {
                logger.LogWarning("{Service} has not started", nameof(MqttCoreService));
                Status = ServiceStatus.Failed;
                return Task.CompletedTask;
            }
            var pollingDevice = mqttCoreService.CurrentDevicesConfig.PhysicalDevices.Where(d => d.UsePolling && d.Enable).ToList();
            pollingDevice.ForEach(d =>
            {
                var timer = new System.Timers.Timer
                {
                    Interval = d.PollingInterval.TotalMilliseconds
                };
                timer.Elapsed += async (_, _) =>
                {
                    await mqttCoreService.PollingAndPublishStatusAsync(d, cancellationToken);
                };
                timer.Start();
                pollingTimers.Add(timer);
            });

            logger.LogInformation("started");
            Status = ServiceStatus.Started;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{nameof(StartAsync)}  failed.");
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
        logger.LogInformation("stopped");
        Status = ServiceStatus.Stoped;
        return Task.CompletedTask;
    }
}
