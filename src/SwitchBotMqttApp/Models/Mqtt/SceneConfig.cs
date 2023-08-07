using Newtonsoft.Json;

namespace SwitchBotMqttApp.Models.Mqtt;


public class SceneConfig : MqttControlBase
{
    public SceneConfig(string sceneId, string commandTopic, string name)
        : base(
            topic: $"homeassistant/scene/{sceneId}/config"
            , stateTopic: null
            , value_template: null
            , commandTopic: commandTopic
            , commandTemplate: null
            , name: name
            , uniqueId: sceneId
            , objectId: sceneId
            , device: null
            , deviceClass: null
            , icon: null)
    {
        PayloadOn = sceneId;
    }
    [JsonProperty("payload_on")]
    public string PayloadOn { get; set; }
}