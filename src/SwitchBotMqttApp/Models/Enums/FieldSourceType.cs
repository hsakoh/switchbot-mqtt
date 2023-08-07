using System.Runtime.Serialization;

namespace SwitchBotMqttApp.Models.Enums;

public enum FieldSourceType
{
    [EnumMember(Value = "status")]
    Status,
    [EnumMember(Value = "webhook")]
    Webhook,
    [EnumMember(Value = "both")]
    Both
}
