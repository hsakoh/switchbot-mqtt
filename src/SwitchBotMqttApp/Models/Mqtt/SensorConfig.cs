using Newtonsoft.Json;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.Mqtt;


public class SensorConfig : MqttEntityBase
{
    public SensorConfig(
        DeviceMqtt device, string key, string name, string objectId, string stateTopic, string attributeTopic
        , string? icon = null
        , SensorDeviceClass? deviceClass = null
        , string? entity_category = null
        , string? state_class = null
        , string? unit_of_measurement = null
        , string? value_template = null)
        : base(
            topic: $"homeassistant/sensor/{objectId}/config"
            , name: name
            , uniqueId: objectId
            , objectId: objectId
            , device: device
            , deviceClass: deviceClass?.ToEnumMemberValue()
            , icon: icon)
    {
        StateTopic = stateTopic;
        JsonAttributesTopic = attributeTopic;
        UnitOfMeasurement = unit_of_measurement;
        StateClass = state_class;
        EntityCategory = entity_category;
        ValueTemplate = value_template ?? $"{{{{value_json.{key}}}}}";
    }
    [JsonProperty("value_template")]
    public string ValueTemplate { get; set; }

    [JsonProperty("state_topic")]
    public string StateTopic { get; set; }
    [JsonProperty("json_attributes_topic")]
    public string JsonAttributesTopic { get; set; }
    [JsonProperty("unit_of_measurement")]
    public string? UnitOfMeasurement { get; set; }
    [JsonProperty("state_class")]
    public string? StateClass { get; set; }
    [JsonProperty("entity_category")]
    public string? EntityCategory { get; set; }
}

