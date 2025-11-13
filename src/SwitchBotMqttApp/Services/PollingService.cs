namespace SwitchBotMqttApp.Services;

/// <summary>
/// Periodically polls device status from SwitchBot API for configured physical devices.
/// Uses timer-based polling intervals configured per device.
/// </summary>
public class PollingService(
    ILogger<PollingService> logger
        , MqttCoreService mqttCoreService) : ManagedServiceBase
{
    private readonly List<System.Timers.Timer> pollingTimers = [];

    /// <summary>
    /// Starts the polling service and creates timers for each device configured to use polling.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Stops the polling service and disposes all active polling timers.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
