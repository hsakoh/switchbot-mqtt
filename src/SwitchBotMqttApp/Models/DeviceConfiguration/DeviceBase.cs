using SwitchBotMqttApp.Models.Enums;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.DeviceConfiguration;

public class DeviceBase
{
    public string DeviceId { get; set; } = default!;
    public string DeviceName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public bool Enable { get; set; }

    [JsonIgnore]
    public ConfigureStatus ConfigureStatus { get; set; } = ConfigureStatus.NoChange;

    public JsonObject? RawValue { get; set; }

    public List<CommandConfig> Commands { get; set; } = default!;
    public DeviceType DeviceType { get; set; } = default!;
}
