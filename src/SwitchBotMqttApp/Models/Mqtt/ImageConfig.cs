using Newtonsoft.Json;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.Mqtt;

public class ImageConfig(
    DeviceMqtt device, string name, string objectId, string imageTopic, string? icon = null)
    : MqttEntityBase(
        topic: $"homeassistant/image/{objectId}/config",
        name: name,
        uniqueId: objectId,
        objectId: objectId,
        component: "image",
        device: device,
        deviceClass: null,
        icon: icon)
{
    [JsonProperty("image_topic")]
    public string ImageTopic { get; set; } = imageTopic;

    [JsonProperty("image_encoding")]
    public string ImageEncoding { get; set; } = "b64";

    [JsonProperty("content_type")]
    public string ContentType { get; set; } = "image/jpeg";
}
