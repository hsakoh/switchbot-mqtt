using Newtonsoft.Json;

namespace SwitchBotMqttApp.Models.Mqtt;

public class MqttControlBase(string topic, string? defaultValue, string commandTopic, string? commandTemplate, string name, string uniqueId, string objectId, DeviceMqtt? device, string? deviceClass, string? icon) : MqttEntityBase(
        topic: topic
            , name: name
            , uniqueId: uniqueId
            , objectId: objectId
            , device: device
            , deviceClass: deviceClass
            , icon: icon)
{
    [JsonProperty("command_topic")]
    public string CommandTopic { get; set; } = commandTopic;

    [JsonProperty("command_template")]
    public string? CommandTemplate { get; set; } = commandTemplate;

    [JsonProperty("value_template")]
    public string? ValueTemplate { get; set; } = defaultValue == null ? null : $"{{{{{defaultValue}}}}}";

}