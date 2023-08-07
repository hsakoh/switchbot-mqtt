using Newtonsoft.Json;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.Mqtt;

public class TextConfig : MqttControlBase
{
    public TextConfig(
        DeviceMqtt device, string objectId, string stateTopic, string commandTopic, string commandTemplate, string name
        , int? min, int? max, TextMode textMode)
    : base(
            topic: $"homeassistant/text/{objectId}/config"
            , stateTopic: stateTopic
            , value_template: null
            , commandTopic: commandTopic
            , commandTemplate: commandTemplate
            , name: name
            , uniqueId: objectId
            , objectId: objectId
            , device: device
            , deviceClass: null
            , icon: null)
    {
        Min = min;
        Max = max;
        Mode = textMode.ToEnumMemberValue()!;
    }

    [JsonProperty("min")]
    public int? Min { get; set; }

    [JsonProperty("max")]
    public int? Max { get; set; }

    [JsonProperty("mode")]
    public string Mode { get; set; }
}
