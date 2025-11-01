using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.Enums;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum PhysicalOrVirtual
{
    PhysicalDevice,
    VirtualInfraredRemoteDevice
}
