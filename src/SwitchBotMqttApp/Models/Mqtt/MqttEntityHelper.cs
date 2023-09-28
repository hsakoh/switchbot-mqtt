using SwitchBotMqttApp.Models.DeviceConfiguration;
using SwitchBotMqttApp.Models.DeviceDefinitions;
using SwitchBotMqttApp.Models.Enums;
using SwitchBotMqttApp.Models.HomeAssistant;
using System.Text.Json;

namespace SwitchBotMqttApp.Models.Mqtt;

public static class MqttEntityHelper
{
    public static string GetStateTopicByType(string deviceId, FieldSourceType fieldSourceType)
    {
        return fieldSourceType switch
        {
            FieldSourceType.Status => GetSensorStateTopic(deviceId),
            FieldSourceType.Webhook => GetWebhookTopic(deviceId),
            _ => GetBothTopic(deviceId),
        };
    }

    public static string GetSensorStateTopic(string deviceId)
    {
        return $"switchbot/{deviceId}/status/{FieldSourceType.Status.ToEnumMemberValue()}";
    }

    public static string GetSensorUpdateTopic(string deviceId)
    {
        return $"switchbot/{deviceId}/status/update";
    }
    public static string GetAttributeTopic(string deviceId)
    {
        return $"switchbot/{deviceId}/attribute";
    }
    public static string GetWebhookTopic(string deviceId)
    {
        return $"switchbot/{deviceId}/status/{FieldSourceType.Webhook.ToEnumMemberValue()}";
    }
    public static string GetBothTopic(string deviceId)
    {
        return $"switchbot/{deviceId}/status/{FieldSourceType.Both.ToEnumMemberValue()}";
    }

    public static string GetCommandTopic(string deviceId)
    {
        return $"switchbot/{deviceId}/cmd";
    }
    public static string GetCommandParamObjectId(string deviceId, int commandIndex, string paramName)
    {
        return $"{paramName}_{commandIndex}_cmd_{deviceId}";
    }

    public static string GetCommandTemplate(CommandConfig commandConfig, string paramName, ParameterType? parameterType = null)
    {
        bool isNumberValue = parameterType switch
        {
            ParameterType.Long => true,
            ParameterType.Range => true,
            _ => false,
        };

        return JsonSerializer.Serialize(new MqttCommandPayload()
        {
            CommandType = commandConfig.CommandType.ToEnumMemberValue()!,
            Command = commandConfig.Command,
            ParamName = paramName,
            ParamValue = isNumberValue ? "{{value}}" : "{{value}}",
        }, JsonSerializerOptions);
    }
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
    };


    public static TextConfig CreateCommandParamTextEntity(DeviceBase deviceConf, int commandIndex, CommandConfig command, DeviceMqtt deviceMqtt, CommandPayloadDefinition paramDef, string defaultValue)
    {
        return new TextConfig(
            deviceMqtt
            , defaultValue: defaultValue
            , objectId: GetCommandParamObjectId(deviceConf.DeviceId, commandIndex, paramDef.Name)
            , commandTopic: GetCommandTopic(deviceConf.DeviceId)
            , commandTemplate: GetCommandTemplate(command, paramDef.Name, paramDef.ParameterType)
            , name: paramDef.Name
            , min: null
            , max: null
            , textMode: TextMode.Text
        );
    }

    public static SelectConfig CreateCommandParamSelectEntity(DeviceBase deviceConf, int commandIndex, CommandConfig command, DeviceMqtt deviceMqtt, CommandPayloadDefinition paramDef, string defaultValue, string additionalObjectId = "")
    {
        return new SelectConfig(
            deviceMqtt
            , defaultValue: defaultValue
            , objectId: GetCommandParamObjectId(deviceConf.DeviceId, commandIndex, paramDef.Name + additionalObjectId)
            , commandTopic: GetCommandTopic(deviceConf.DeviceId)
            , commandTemplate: GetCommandTemplate(command, paramDef.Name, paramDef.ParameterType)
            , name: paramDef.Name
            , options: paramDef.GetOptionsDescription()!
        );
    }

    public static NumberConfig CreateCommandParamNumberEntity(DeviceBase deviceConf, int commandIndex, CommandConfig command, DeviceMqtt deviceMqtt, CommandPayloadDefinition paramDef, long? min, long? max, NumberMode numberMode,string? defaultValue, string additionalObjectId = "")
    {
        return new NumberConfig(
            deviceMqtt
            , defaultValue: defaultValue
            , objectId: GetCommandParamObjectId(deviceConf.DeviceId, commandIndex, paramDef.Name + additionalObjectId)
            , commandTopic: GetCommandTopic(deviceConf.DeviceId)
            , commandTemplate: GetCommandTemplate(command, paramDef.Name, paramDef.ParameterType)
            , name: paramDef.Name
            , deviceClass: NumberDeviceClass.None
            , min: min
            , max: max
            , numberMode: numberMode
            , unitOfMeasurement: null
        );
    }

    public static ButtonConfig CreateCommandButtonEntity(DeviceMqtt deviceMqtt, DeviceBase deviceConf, int commandIndex, CommandConfig command, CommandDefinition commandDef)
    {
        return new ButtonConfig(
            deviceMqtt
            , objectId: GetCommandParamObjectId(deviceConf.DeviceId, commandIndex, ButtonPrefix)
            , commandTopic: GetCommandTopic(deviceConf.DeviceId)
            , commandTemplate: GetCommandTemplate(command, ButtonPrefix)
            , payloadPress: "action"
            , name: command.CommandType switch
            {
                CommandType.Command => command.Command,
                CommandType.Customize => string.IsNullOrEmpty(command.DisplayName) ? $"Customize ({command.Command})" : command.DisplayName,
                _ => string.IsNullOrEmpty(command.DisplayName) ? $"Tag ({command.Command})" : command.DisplayName,

            }
            , deviceClass: commandDef.ButtonDeviceClass
            , icon: commandDef.Icon
        );
    }

    public static ButtonConfig CreateSensorUpdateButtonEntity(DeviceMqtt deviceMqtt, PhysicalDevice physicalDevice)
    {
        return new ButtonConfig(
            deviceMqtt
            , objectId: $"update_{physicalDevice.DeviceId}"
            , commandTopic: GetSensorUpdateTopic(physicalDevice.DeviceId)
            , commandTemplate: null
            , payloadPress: "update"
            , name: "Polling"
            , deviceClass: ButtonDeviceClass.Restart
            , icon: "mdi:refresh"
        );
    }

    internal static SceneConfig CreateSceneEntity(string sceneName, string sceneId)
    {
        return new SceneConfig(
            sceneId: sceneId
            , commandTopic: GetSceneCommandTopic()
            , name: sceneName
        );
    }

    internal static string GetSceneCommandTopic()
    {
        return $"switchbot/scene/execute";
    }

    public const string ButtonPrefix = "btn";
}
