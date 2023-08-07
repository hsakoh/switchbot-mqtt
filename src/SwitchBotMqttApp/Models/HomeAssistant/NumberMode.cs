using System.Runtime.Serialization;

namespace SwitchBotMqttApp.Models.HomeAssistant;


public enum NumberMode
{
    [EnumMember(Value = "slider")]
    Slider,
    [EnumMember(Value = "box")]
    Box,
}