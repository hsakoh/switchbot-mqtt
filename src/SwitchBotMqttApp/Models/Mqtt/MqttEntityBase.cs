using Newtonsoft.Json;

namespace SwitchBotMqttApp.Models.Mqtt;

public class MqttEntityBase(string topic, string name, string uniqueId, string objectId,string component,DeviceMqtt? device, string? deviceClass, string? icon)
{
    [JsonIgnore]
    public string Topic { get; set; } = topic;

    [JsonProperty("name")]
    public string Name { get; set; } = name;
    [JsonProperty("unique_id")]
    public string UniqueId { get; set; } = uniqueId;
    [JsonProperty("object_id")]
    public string ObjectId { get; set; } = objectId;
    [JsonProperty("default_entity_id")]
    public string DefaultEntityId { get; set; } = $"{component}.{objectId}";
    [JsonProperty("device")]
    public DeviceMqtt? Device { get; set; } = device;

    [JsonProperty("device_class")]
    public string? DeviceClass { get; set; } = deviceClass;

    [JsonProperty("icon")]
    public string? Icon { get; set; } = icon;

}