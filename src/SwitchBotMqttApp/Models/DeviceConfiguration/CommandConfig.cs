using SwitchBotMqttApp.Models.Enums;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.DeviceConfiguration;

public class CommandConfig
{
    public bool Enable { get; set; }
    public CommandType CommandType { get; set; } = default!;
    public string Command { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
}