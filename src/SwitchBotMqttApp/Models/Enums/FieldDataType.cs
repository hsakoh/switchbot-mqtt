using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FieldDataType
{
    Boolean,
    Float,
    Integer,
    Long,
    String,
}