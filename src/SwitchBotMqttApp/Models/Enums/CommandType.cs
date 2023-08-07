using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.Enums;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum CommandType
{
    [EnumMember(Value = "command")]
    Command,
    [EnumMember(Value = "customize")]
    Customize,
    [EnumMember(Value = "tag")]
    Tag,
}