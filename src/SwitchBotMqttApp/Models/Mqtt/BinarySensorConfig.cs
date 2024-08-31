using Newtonsoft.Json;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.Mqtt;


public class BinarySensorConfig(
    DeviceMqtt device, string key, string name, string objectId, string stateTopic
        , BinarySensorDeviceClass deviceClass
        , object payloadOn
        , object payloadOff
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
    public string ValueTemplate { get; set; } = value_template ?? $"{{{{ value_json['{key}'] if (value_json['{key}'] is defined and value_json['{key}'] is not none) else states('sensor.{objectId}') }}}}";

    [JsonProperty("state_topic")]
    public string StateTopic { get; set; } = stateTopic;
    [JsonProperty("payload_on")]
    public object PayloadOn { get; set; } = payloadOn;
    [JsonProperty("payload_off")]
    public object PayloadOff { get; set; } = payloadOff;
}

