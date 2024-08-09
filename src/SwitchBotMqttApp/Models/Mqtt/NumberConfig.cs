using Newtonsoft.Json;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.Mqtt;

public class NumberConfig(
    DeviceMqtt device, string objectId, string? defaultValue, string commandTopic, string commandTemplate, string name, NumberDeviceClass deviceClass
        , decimal? min, decimal? max, NumberMode numberMode, string? unitOfMeasurement) : MqttControlBase(
        topic: $"homeassistant/number/{objectId}/config"
            , defaultValue: defaultValue 
            , commandTopic: commandTopic
            , commandTemplate: commandTemplate
            , name: name
            , uniqueId: objectId
            , objectId: objectId
            , device: device
            , deviceClass: deviceClass.ToEnumMemberValue()
            , icon: null)
{
    [JsonProperty("min")]
    public decimal? Min { get; set; } = min;

    [JsonProperty("max")]
    public decimal? Max { get; set; } = max;

    [JsonProperty("mode")]
    public string Mode { get; set; } = numberMode.ToEnumMemberValue()!;

    [JsonProperty("unit_of_measurement")]
    public string? UnitOfMeasurement { get; set; } = unitOfMeasurement;
}