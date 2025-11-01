using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.HomeAssistant;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum TextMode
{
    [EnumMember(Value = "text")]
    Text,
    [EnumMember(Value = "password")]
    Password,
}