using Newtonsoft.Json;

namespace SwitchBotMqttApp.Models.Mqtt;


public class SceneConfig(string sceneId, string commandTopic, string name) : MqttControlBase(
        topic: $"homeassistant/scene/{sceneId}/config"
            , defaultValue: null
            , commandTopic: commandTopic
            , commandTemplate: null
            , name: name
            , uniqueId: sceneId
            , objectId: sceneId
            , device: null
            , deviceClass: null
            , icon: null)
{
    [JsonProperty("payload_on")]
    public string PayloadOn { get; set; } = sceneId;
}