using Newtonsoft.Json;

namespace SwitchBotMqttApp.Models.Mqtt;


public class SelectConfig : MqttControlBase
{
    public SelectConfig(
        DeviceMqtt device, string objectId, string defaultValue, string commandTopic, string commandTemplate, string name, string[] options)
    : base(
            topic: $"homeassistant/select/{objectId}/config"
            , defaultValue: defaultValue == null ? null : $"\"{defaultValue}\""
            , commandTopic: commandTopic
            , commandTemplate: commandTemplate
            , name: name
            , uniqueId: objectId
            , objectId: objectId
            , device: device
            , deviceClass: null
            , icon: null)
    {
        Options = options;
    }

    [JsonProperty("options")]
    public string[] Options { get; set; }
}