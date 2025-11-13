using SwitchBotMqttApp.Models.Enums;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.DeviceConfiguration;

/// <summary>
/// Base class for device configuration containing common properties for physical and virtual IR remote devices.
/// </summary>
public class DeviceBase
{
    /// <summary>
    /// Gets or sets the unique device identifier (MAC address for physical devices).
    /// </summary>
    public string DeviceId { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the user-friendly device name.
    /// </summary>
    public string DeviceName { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the device description for user notes.
    /// </summary>
    public string Description { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets a value indicating whether this device is enabled for MQTT integration.
    /// </summary>
    public bool Enable { get; set; }

    /// <summary>
    /// Gets or sets the configuration status tracking changes from API sync.
    /// Not persisted to JSON file.
    /// </summary>
    [JsonIgnore]
    public ConfigureStatus ConfigureStatus { get; set; } = ConfigureStatus.NoChange;

    /// <summary>
    /// Gets or sets the raw JSON value from SwitchBot API response for this device.
    /// Contains additional device metadata not mapped to strongly-typed properties.
    /// </summary>
    public JsonObject? RawValue { get; set; }

    /// <summary>
    /// Gets or sets the list of commands available for this device.
    /// </summary>
    public List<CommandConfig> Commands { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the device type (e.g., Bot, Curtain, Hub, AirConditioner).
    /// </summary>
    public DeviceType DeviceType { get; set; } = default!;
}
