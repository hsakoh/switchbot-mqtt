using SwitchBotMqttApp.Models.Enums;

namespace SwitchBotMqttApp.Models.DeviceDefinitions;

/// <summary>
/// Defines metadata and capabilities for a SwitchBot device type.
/// Loaded from JSON files in the MasterData directory.
/// </summary>
public class DeviceDefinition
{
    /// <summary>
    /// Gets or sets the device type enum value.
    /// </summary>
    public DeviceType DeviceType { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the device type string as returned by SwitchBot API.
    /// </summary>
    public string ApiDeviceTypeString { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the device type string for customized IR remote devices.
    /// </summary>
    public string? CustomizedDeviceTypeString { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the device type string as sent in webhook notifications.
    /// May differ from API device type string for some devices.
    /// </summary>
    public string? WebhookDeviceTypeString { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets whether this is a physical device or virtual IR remote device.
    /// </summary>
    public PhysicalOrVirtual PhysicalOrVirtual { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets a value indicating whether this device type supports status polling.
    /// </summary>
    public bool IsSupportPolling { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets a value indicating whether this device type supports webhook notifications.
    /// </summary>
    public bool IsSupportWebhook { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets a value indicating whether this device type supports command execution.
    /// </summary>
    public bool IsSupportCommand { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the device description.
    /// </summary>
    public string? Description { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the user-friendly display name for this device type.
    /// </summary>
    public string? DisplayName { get; set; } = default!;

    /// <summary>
    /// Gets or sets the array of status fields (sensors) available for this device type.
    /// </summary>
    public FieldDefinition[] Fields { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the array of commands available for this device type.
    /// </summary>
    public CommandDefinition[] Commands { get; set; } = default!;
}
