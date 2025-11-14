using Newtonsoft.Json;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.Mqtt;


public class SensorConfig(
    DeviceMqtt device, string key, string name, string objectId, string stateTopic
        , string? icon = null
        , SensorDeviceClass? deviceClass = null
        , string? entity_category = null
        , string? state_class = null
        , string? unit_of_measurement = null
        , string? value_template = null) : MqttEntityBase(
        topic: $"homeassistant/sensor/{objectId}/config"
            , name: name
            , uniqueId: objectId
            , objectId: objectId
            , component : "sensor"
            , device: device
            , deviceClass: deviceClass?.ToEnumMemberValue()
            , icon: icon)
{
    [JsonProperty("value_template")]
    public string ValueTemplate { get; set; } = value_template ?? $"{{{{ value_json['{key}'] if (value_json['{key}'] is defined and value_json['{key}'] is not none) else states('sensor.{objectId}') }}}}";

    [JsonProperty("state_topic")]
    public string StateTopic { get; set; } = stateTopic;
    [JsonProperty("unit_of_measurement")]
    public string? UnitOfMeasurement { get; set; } = unit_of_measurement;
    [JsonProperty("state_class")]
    public string? StateClass { get; set; } = state_class;
    [JsonProperty("entity_category")]
    public string? EntityCategory { get; set; } = entity_category;
}

