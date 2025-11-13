namespace SwitchBotMqttApp.Services;

/// <summary>
/// Base class for managed services with lifecycle tracking.
/// Implements <see cref="IHostedService"/> for integration with ASP.NET Core hosting.
/// </summary>
public abstract class ManagedServiceBase : IHostedService
{
    /// <summary>
    /// Gets or sets the current status of the service.
    /// </summary>
    public ServiceStatus Status { get; set; } = ServiceStatus.Initial;

    /// <summary>
    /// Starts the service asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public abstract Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the service asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public abstract Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Represents the lifecycle status of a managed service.
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>Service has been created but not started.</summary>
        Initial,
        /// <summary>Service is currently starting.</summary>
        Starting,
        /// <summary>Service has started successfully and is running.</summary>
        Started,
        /// <summary>Service has been stopped.</summary>
        Stoped,
        /// <summary>Service failed to start or encountered an error.</summary>
        Failed,
        /// <summary>Service is disabled and will not start.</summary>
        Disabled,
    }
}
