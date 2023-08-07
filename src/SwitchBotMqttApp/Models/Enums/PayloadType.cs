using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PayloadType
{
    Default,
    SingleValue,
    Json,
    JoinColon,
    JoinComma,
    JoinSemiColon,
}