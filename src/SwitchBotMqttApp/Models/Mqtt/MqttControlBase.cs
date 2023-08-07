using Newtonsoft.Json;

namespace SwitchBotMqttApp.Models.Mqtt;

public class MqttControlBase : MqttEntityBase
{
    public MqttControlBase(string topic, string? stateTopic, string? value_template, string commandTopic, string? commandTemplate, string name, string uniqueId, string objectId, DeviceMqtt? device, string? deviceClass, string? icon)
    : base(
            topic: topic
            , name: name
            , uniqueId: objectId
            , objectId: objectId
            , device: device
            , deviceClass: deviceClass
            , icon: icon)
    {
        CommandTopic = commandTopic;
        CommandTemplate = commandTemplate;
        StateTopic = stateTopic;
        ValueTemplate = stateTopic == null ? null : value_template ?? $"{{{{value_json.{name}}}}}";
    }

    [JsonProperty("command_topic")]
    public string CommandTopic { get; set; }

    [JsonProperty("command_template")]
    public string? CommandTemplate { get; set; }

    [JsonProperty("value_template")]
    public string? ValueTemplate { get; set; }

    [JsonProperty("state_topic")]
    public string? StateTopic { get; set; }

}