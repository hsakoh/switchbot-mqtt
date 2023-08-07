using System.Runtime.Serialization;

namespace SwitchBotMqttApp.Models.HomeAssistant;

public enum TextMode
{
    [EnumMember(Value = "text")]
    Text,
    [EnumMember(Value = "password")]
    Password,
}