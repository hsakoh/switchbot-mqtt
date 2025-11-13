namespace HomeAssistantAddOn.Mqtt;

/// <summary>
/// Configuration options for MQTT broker connection.
/// Supports both auto-configuration from Home Assistant Supervisor and manual configuration.
/// </summary>
public class MqttOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to auto-configure MQTT settings from Home Assistant Supervisor.
    /// When true, MQTT connection details are retrieved from Supervisor API.
    /// </summary>
    public bool AutoConfig { get; set; } = true;

    /// <summary>
    /// Gets or sets the MQTT broker host address.
    /// Used when AutoConfig is false.
    /// </summary>
    public string? Host { get; set; } = null;

    /// <summary>
    /// Gets or sets the MQTT broker port number.
    /// Default is 1883 (unencrypted) or 8883 (TLS).
    /// </summary>
    public int Port { get; set; } = 1883;

    /// <summary>
    /// Gets or sets the MQTT client username for authentication.
    /// Used when AutoConfig is false.
    /// </summary>
    public string? Id { get; set; } = null;

    /// <summary>
    /// Gets or sets the MQTT client password for authentication.
    /// Used when AutoConfig is false.
    /// </summary>
    public string? Pw { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether to use TLS/SSL encryption for MQTT connection.
    /// Used when AutoConfig is false.
    /// </summary>
    public bool Tls { get; set; } = false;
}