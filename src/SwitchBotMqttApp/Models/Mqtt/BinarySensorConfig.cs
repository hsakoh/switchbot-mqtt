using Newtonsoft.Json;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.Mqtt;


public class BinarySensorConfig(
    DeviceMqtt device, string key, string name, string objectId, string stateTopic, string attributeTopic
        , BinarySensorDeviceClass deviceClass
        , string payloadOn
        , string payloadOff
        , string? icon = null
        , string? value_template = null) : MqttEntityBase(
        topic: $"homeassistant/binary_sensor/{objectId}/config"
            , name: name
            , uniqueId: objectId
            , objectId: objectId
            , device: device
            , deviceClass: deviceClass.ToEnumMemberValue()
            , icon: icon)
{
    [JsonProperty("value_template")]
    public string ValueTemplate { get; set; } = value_template ?? $"{{{{value_json.{key}}}}}";

    [JsonProperty("state_topic")]
    public string StateTopic { get; set; } = stateTopic;
    [JsonProperty("json_attributes_topic")]
    public string JsonAttributesTopic { get; set; } = attributeTopic;
    [JsonProperty("payload_on")]
    public string PayloadOn { get; set; } = payloadOn;
    [JsonProperty("payload_off")]
    public string PayloadOff { get; set; } = payloadOff;
}

