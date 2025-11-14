using Newtonsoft.Json;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.Mqtt;

public class TextConfig(
    DeviceMqtt device, string objectId, string? defaultValue, string commandTopic, string commandTemplate, string name
        , int? min, int? max, TextMode textMode) : MqttControlBase(
        topic: $"homeassistant/text/{objectId}/config"
            , defaultValue: defaultValue == null ? null : $"\"{defaultValue}\""
            , commandTopic: commandTopic
            , commandTemplate: commandTemplate
            , name: name
            , uniqueId: objectId
            , objectId: objectId
            , component : "text"
            , device: device
            , deviceClass: null
            , icon: null)
{
    [JsonProperty("min")]
    public int? Min { get; set; } = min;

    [JsonProperty("max")]
    public int? Max { get; set; } = max;

    [JsonProperty("mode")]
    public string Mode { get; set; } = textMode.ToEnumMemberValue()!;
}
