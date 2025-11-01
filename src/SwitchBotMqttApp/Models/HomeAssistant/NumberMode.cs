using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.HomeAssistant;


[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum NumberMode
{
    [EnumMember(Value = "slider")]
    Slider,
    [EnumMember(Value = "box")]
    Box,
}