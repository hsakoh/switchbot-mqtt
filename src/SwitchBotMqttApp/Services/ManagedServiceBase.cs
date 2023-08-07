namespace SwitchBotMqttApp.Services;

public abstract class ManagedServiceBase : IHostedService
{
    public ServiceStatus Status { get; set; } = ServiceStatus.Initial;

    public abstract Task StartAsync(CancellationToken cancellationToken = default);

    public abstract Task StopAsync(CancellationToken cancellationToken = default);

    public enum ServiceStatus
    {
        Initial,
        Starting,
        Started,
        Stoped,
        Failed,
        Disabled,
    }
}
