using SwitchBotMqttApp.Models.Enums;

namespace SwitchBotMqttApp.Models.DeviceDefinitions;

public class KeySetDefinition
{
    public DeviceType DeviceType { get; set; } = default!;
    public string KeyName { get; set; } = default!;
    public string KeyTag { get; set; } = default!;
}
