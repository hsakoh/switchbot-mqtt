using Newtonsoft.Json;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.Mqtt;


public class BinarySensorConfig : MqttEntityBase
{
    public BinarySensorConfig(
        DeviceMqtt device, string key, string name, string objectId, string stateTopic, string attributeTopic
        , BinarySensorDeviceClass deviceClass
        , string payloadOn
        , string payloadOff
        , string? icon = null
        , string? value_template = null)
        : base(
            topic: $"homeassistant/binary_sensor/{objectId}/config"
            , name: name
            , uniqueId: objectId
            , objectId: objectId
            , device: device
            , deviceClass: deviceClass.ToEnumMemberValue()
            , icon: icon)
    {
        StateTopic = stateTopic;
        JsonAttributesTopic = attributeTopic;
        ValueTemplate = value_template ?? $"{{{{value_json.{key}}}}}";
        PayloadOff = payloadOff;
        PayloadOn = payloadOn;
    }
    [JsonProperty("value_template")]
    public string ValueTemplate { get; set; }

    [JsonProperty("state_topic")]
    public string StateTopic { get; set; }
    [JsonProperty("json_attributes_topic")]
    public string JsonAttributesTopic { get; set; }
    [JsonProperty("payload_on")]
    public string PayloadOn { get; set; }
    [JsonProperty("payload_off")]
    public string PayloadOff { get; set; }
}

