namespace SwitchBotMqttApp.Models.DeviceConfiguration;

/// <summary>
/// Configuration for virtual infrared remote devices that control IR appliances via SwitchBot Hub.
/// Includes support for both predefined and customized IR remote types.
/// </summary>
public class VirtualInfraredRemoteDevice : DeviceBase
{
    /// <summary>
    /// Gets or sets a value indicating whether this IR remote uses custom button mappings
    /// instead of a predefined device type template.
    /// </summary>
    public bool IsCustomized { get; set; } = default!;
}