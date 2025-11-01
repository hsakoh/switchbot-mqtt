using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.HomeAssistant;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ButtonDeviceClass
{
    /// <summary>
    /// Generic button. This is the default and doesn’t need to be set.
    /// </summary>
    None,
    /// <summary>
    /// The button is used to identify a device.
    /// </summary>
    [EnumMember(Value = "identify")]
    Identify,
    /// <summary>
    /// The button restarts the device.
    /// </summary>
    [EnumMember(Value = "restart")]
    Restart,
    /// <summary>
    /// The button updates the software of the device.
    /// </summary>
    [EnumMember(Value = "update")]
    Update,
}