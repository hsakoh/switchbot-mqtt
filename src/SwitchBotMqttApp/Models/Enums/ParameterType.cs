using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ParameterType
{
    Long,
    Range,
    Select,
    SelectOrRange,
    String,
}