using Newtonsoft.Json;

namespace SwitchBotMqttApp.Models.Mqtt;


public class SelectConfig(
    DeviceMqtt device, string objectId, string defaultValue, string commandTopic, string commandTemplate, string name, string[] options) : MqttControlBase(
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
    [JsonProperty("options")]
    public string[] Options { get; set; } = options;
}