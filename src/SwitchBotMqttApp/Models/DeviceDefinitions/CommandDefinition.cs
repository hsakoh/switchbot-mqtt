using SwitchBotMqttApp.Models.Enums;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.DeviceDefinitions;

public class CommandDefinition
{
    public DeviceType DeviceType { get; set; } = default!;
    public CommandType CommandType { get; set; } = default!;
    public string Command { get; set; } = default!;
    public PayloadType PayloadType { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Icon { get; set; } = default!;
    public ButtonDeviceClass? ButtonDeviceClass { get; set; } = default!;
    public string? DisplayName { get; set; } = default!;
    public string? DisplayNameJa { get; set; } = default!;

}
