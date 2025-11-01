using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.Enums;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum FieldSourceType
{
    [EnumMember(Value = "status")]
    Status,
    [EnumMember(Value = "webhook")]
    Webhook,
    [EnumMember(Value = "both")]
    Both
}
