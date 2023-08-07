using Newtonsoft.Json;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.Mqtt;

public class NumberConfig : MqttControlBase
{
    public NumberConfig(
        DeviceMqtt device, string objectId,string stateTopic, string commandTopic, string commandTemplate, string name, NumberDeviceClass deviceClass
        , decimal? min, decimal? max, NumberMode numberMode, string? unitOfMeasurement)
    : base(
            topic: $"homeassistant/number/{objectId}/config"
            , stateTopic: stateTopic
            , value_template: null
            , commandTopic: commandTopic
            , commandTemplate: commandTemplate
            , name: name
            , uniqueId: objectId
            , objectId: objectId
            , device: device
            , deviceClass: deviceClass.ToEnumMemberValue()
            , icon: null)
    {
        Min = min;
        Max = max;
        Mode = numberMode.ToEnumMemberValue()!;
        UnitOfMeasurement = unitOfMeasurement;
    }

    [JsonProperty("min")]
    public decimal? Min { get; set; }

    [JsonProperty("max")]
    public decimal? Max { get; set; }

    [JsonProperty("mode")]
    public string Mode { get; set; }

    [JsonProperty("unit_of_measurement")]
    public string? UnitOfMeasurement { get; set; }
}