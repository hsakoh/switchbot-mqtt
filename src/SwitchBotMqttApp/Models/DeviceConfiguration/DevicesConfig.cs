using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.DeviceConfiguration;

public class DevicesConfig
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<PhysicalDevice> PhysicalDevices { get; set; } = new List<PhysicalDevice>();
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<VirtualInfraredRemoteDevice> VirtualInfraredRemoteDevices { get; set; } = new List<VirtualInfraredRemoteDevice>();
}
