using Newtonsoft.Json;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.Mqtt;


public class ButtonConfig : MqttControlBase
{
    public ButtonConfig(
        DeviceMqtt device, string objectId, string commandTopic, string? commandTemplate, string payloadPress, string name, ButtonDeviceClass? deviceClass, string? icon)
        : base(
            topic: $"homeassistant/button/{objectId}/config"
            , defaultValue: null
            , commandTopic: commandTopic
            , commandTemplate: commandTemplate
            , name: name
            , uniqueId: objectId
            , objectId: objectId
            , device: device
            , deviceClass: deviceClass?.ToEnumMemberValue()
            , icon: icon)
    {
        PayloadPress = payloadPress;
    }
    [JsonProperty("payload_press")]
    public string PayloadPress { get; set; }
}