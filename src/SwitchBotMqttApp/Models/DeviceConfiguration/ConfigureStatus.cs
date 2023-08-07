using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.DeviceConfiguration;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConfigureStatus
{
    NoChange,
    Missing,
    Deleting,
    New,
    Modified,
}