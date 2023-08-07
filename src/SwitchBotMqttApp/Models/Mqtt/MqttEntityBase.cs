using Newtonsoft.Json;

namespace SwitchBotMqttApp.Models.Mqtt;

public class MqttEntityBase
{
    public MqttEntityBase(string topic, string name, string uniqueId, string objectId, DeviceMqtt? device, string? deviceClass, string? icon)
    {
        Topic = topic;
        Name = name;
        UniqueId = uniqueId;
        ObjectId = objectId;
        Device = device;
        DeviceClass = deviceClass;
        Icon = icon;
    }

    [JsonIgnore]
    public string Topic { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("unique_id")]
    public string UniqueId { get; set; }
    [JsonProperty("object_id")]
    public string ObjectId { get; set; }
    [JsonProperty("device")]
    public DeviceMqtt? Device { get; set; }

    [JsonProperty("device_class")]
    public string? DeviceClass { get; set; }

    [JsonProperty("icon")]
    public string? Icon { get; set; }

}