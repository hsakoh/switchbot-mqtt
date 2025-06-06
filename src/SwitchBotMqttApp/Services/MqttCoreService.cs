using HomeAssistantAddOn.Mqtt;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Options;
using SwitchBotMqttApp.Configurations;
using SwitchBotMqttApp.Logics;
using SwitchBotMqttApp.Models.DeviceConfiguration;
using SwitchBotMqttApp.Models.DeviceDefinitions;
using SwitchBotMqttApp.Models.Enums;
using SwitchBotMqttApp.Models.HomeAssistant;
using SwitchBotMqttApp.Models.Mqtt;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SwitchBotMqttApp.Services;

public class MqttCoreService(
    ILogger<MqttCoreService> logger
        , DeviceConfigurationManager deviceConfigurationManager
        , DeviceDefinitionsManager deviceDefinitionsManager
        , MqttService mqttService
        , SwitchBotApiClient switchBotApiClient
        , IOptions<MessageRetainOptions> messageRetailOptions
        , DeviceStatePersistanceManager deviceStatePersistanceManager) : ManagedServiceBase
{
    public DevicesConfig CurrentDevicesConfig { get; set; } = default!;

    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Status = ServiceStatus.Starting;
            CurrentDevicesConfig = await deviceConfigurationManager.GetFileAsync(cancellationToken);
            CommandPayloadDictionary.Clear();

            await mqttService.StartAsync();

            foreach (var deviceConf in CurrentDevicesConfig.PhysicalDevices.Where(d => d.Enable).Select(d => (DeviceBase)d)
                                            .Union(CurrentDevicesConfig.VirtualInfraredRemoteDevices.Where(d => d.Enable)))
            {
                var deviceDef = deviceDefinitionsManager.DeviceDefinitions.First(d => d.DeviceType == deviceConf.DeviceType);
                var deviceMqtt = new DeviceMqtt(
                    identifiers: [deviceConf.DeviceId],
                    name: deviceConf.DeviceName,
                    manufacturer: $"SwitchBot{(deviceConf is VirtualInfraredRemoteDevice ? "(VirtualInfraredRemoteDevice)" : "")}",
                    model: deviceDef.ApiDeviceTypeString);


                if (deviceConf is PhysicalDevice physicalDevice
                    && physicalDevice.Fields.Any(s => s.Enable))
                {
                    await PublishFieldEntities(deviceDef, deviceMqtt, physicalDevice, cancellationToken);

                }
                await PublishCommandEntities(deviceMqtt, deviceConf);
            }

            await PublishSceneEntities(cancellationToken);

            logger.LogInformation("started");
            Status = ServiceStatus.Started;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{nameof(StartAsync)}  failed.");
            Status = ServiceStatus.Failed;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await mqttService.StopAsync();
        logger.LogInformation("stopped");
        Status = ServiceStatus.Stoped;
        CommandPayloadDictionary.Clear();
    }

    private async Task PublishFieldEntities(DeviceDefinition deviceDef, DeviceMqtt deviceMqtt, PhysicalDevice physicalDevice, CancellationToken cancellationToken)
    {
        var fieldDefs = deviceDefinitionsManager.FieldDefinitions.Where(f => f.DeviceType == physicalDevice.DeviceType);
        var statusEntities = physicalDevice.Fields.Where(s => s.Enable)
            .Select(s =>
            {
                var fieldDef = fieldDefs.Where(f => f.FieldName == s.FieldName).First();
                if (fieldDef.IsBinary)
                {
                    return (MqttEntityBase)new BinarySensorConfig(
                        deviceMqtt
                        , key: fieldDef.FieldName
                        , name: fieldDef.DisplayName ?? fieldDef.FieldName
                        , objectId: $"{fieldDef.FieldSourceType.ToEnumMemberValue()!}_{fieldDef.FieldName}_{deviceMqtt.Identifiers[0]}"
                        , stateTopic: MqttEntityHelper.GetStateTopic(deviceMqtt.Identifiers[0])
                        , icon: fieldDef.Icon
                        , deviceClass: fieldDef.BinarySensorDeviceClass!.Value
                        , payloadOn: fieldDef.FieldDataType == FieldDataType.Boolean ? bool.Parse(fieldDef.OnValue!) : fieldDef.OnValue!
                        , payloadOff: fieldDef.FieldDataType == FieldDataType.Boolean ? bool.Parse(fieldDef.OffValue!) : fieldDef.OffValue!
                        , value_template: null
                    );
                }
                else
                {
                    string? value_template = null;
                    if (fieldDef.FieldName == "timeOfSample")
                    {
                        value_template = UnixTimeValueTemplateFormat.Replace("%FIELD%", "timeOfSample");
                    }
                    return new SensorConfig(
                        deviceMqtt
                        , key: fieldDef.FieldName
                        , name: fieldDef.DisplayName ?? fieldDef.FieldName
                        , objectId: $"{fieldDef.FieldSourceType.ToEnumMemberValue()!}_{fieldDef.FieldName}_{deviceMqtt.Identifiers[0]}"
                        , stateTopic: MqttEntityHelper.GetStateTopic(deviceMqtt.Identifiers[0])
                        , icon: fieldDef.Icon
                        , deviceClass: fieldDef.SensorDeviceClass ?? SensorDeviceClass.None
                        , entity_category: fieldDef.EntityCategory
                        , state_class: fieldDef.StateClass
                        , unit_of_measurement: fieldDef.UnitOfMeasurement
                        , value_template: value_template
                    );
                }

            }).ToList();
        foreach (var e in statusEntities)
        {
            await PublishEntityAsync(e);
        }

        var statusTimestamp = new SensorConfig(deviceMqtt
            , key: "status_timestamp"
            , name: "Status Lastupdate"
            , objectId: $"{FieldSourceType.Status.ToEnumMemberValue()!}_status_timestamp_{deviceMqtt.Identifiers[0]}"
            , stateTopic: MqttEntityHelper.GetStateTopic(deviceMqtt.Identifiers[0])
            , deviceClass: SensorDeviceClass.Timestamp
            , value_template: UnixTimeValueTemplateFormat.Replace("%FIELD%", "status_timestamp")
            );
        await PublishEntityAsync(statusTimestamp);

        if (deviceDef.IsSupportWebhook && physicalDevice.UseWebhook)
        {
            var webhookTimestamp = new SensorConfig(deviceMqtt
                , key: "webhook_timestamp"
                , name: "Webhook Lastupdate"
                , objectId: $"{FieldSourceType.Webhook.ToEnumMemberValue()!}_webhook_timestamp_{deviceMqtt.Identifiers[0]}"
                , stateTopic: MqttEntityHelper.GetStateTopic(deviceMqtt.Identifiers[0])
                , deviceClass: SensorDeviceClass.Timestamp
                , value_template: UnixTimeValueTemplateFormat.Replace("%FIELD%", "webhook_timestamp")
            );
            await PublishEntityAsync(webhookTimestamp);
        }

        //subscribe update action
        await PublishEntityAsync(MqttEntityHelper.CreateSensorUpdateButtonEntity(deviceMqtt, physicalDevice));
        mqttService.Subscribe(MqttEntityHelper.GetSensorUpdateTopic(physicalDevice.DeviceId), async (payload) => await PollingAndPublishStatusAsync(physicalDevice, CancellationToken.None));


        //publish inital status
        await PollingAndPublishStatusAsync(physicalDevice, cancellationToken);
    }

    private const string UnixTimeValueTemplateFormat = "{% set ts = value_json.get('%FIELD%', {})  %} {% if ts %}\n  {{ (ts / 1000) | timestamp_local | as_datetime }}\n{% else %}\n  {{ this.state }}\n{% endif %}";

    private async Task PublishEntityAsync(MqttEntityBase payload)
    {
        await mqttService.PublishAsync(payload.Topic, payload, messageRetailOptions.Value.Entity);
    }

    private async Task PublishCommandEntities(DeviceMqtt deviceMqtt, DeviceBase deviceConf)
    {
        List<ButtonConfig> buttonEntities = [];
        List<DeviceMqtt> commandDeviceEntities = [];
        List<NumberConfig> numberEntities = [];
        List<SelectConfig> selectEntities = [];
        List<TextConfig> textEntities = [];
        int commandIndex = 0;
        foreach (var command in deviceConf.Commands.Where(c => c.Enable))
        {
            commandIndex++;
            CommandDefinition commandDef;
            if (command.CommandType == CommandType.Command)
            {
                //Difiend command
                commandDef = deviceDefinitionsManager.CommandDefinitions.First(c => c.DeviceType == deviceConf.DeviceType && c.CommandType == command.CommandType && c.Command == command.Command);
            }
            else
            {
                //Costmize or Tag command
                commandDef = new CommandDefinition()
                {
                    DeviceType = deviceConf.DeviceType,
                    CommandType = command.CommandType,
                    Command = command.Command,
                    ButtonDeviceClass = ButtonDeviceClass.None,
                    Description = command.DisplayName,
                    DisplayName = command.DisplayName,
                    DisplayNameJa = command.DisplayName,
                    Icon = null,
                    PayloadType = PayloadType.Default,
                };
            }

            // Keypad deleteKey
            if ((deviceConf.DeviceType == DeviceType.Keypad
                || deviceConf.DeviceType == DeviceType.KeypadTouch
                || deviceConf.DeviceType == DeviceType.KeypadVision)
                && command.Command == "deleteKey")
            {
                var deviceMqttForCommand = new DeviceMqtt(
                        identifiers: [$"{deviceConf.DeviceId}{command.Command}"],
                        name: $"{deviceConf.DeviceName}-{command.Command}",
                        manufacturer: deviceMqtt.Manufacturer,
                        model: deviceMqtt.Model);
                commandDeviceEntities.Add(deviceMqttForCommand);
                buttonEntities.Add(MqttEntityHelper.CreateCommandButtonEntity(deviceMqttForCommand, deviceConf, commandIndex, command, commandDef));
                buttonEntities.Add(MqttEntityHelper.CreateKeypadReloadButtonEntity(deviceMqttForCommand, deviceConf, commandIndex, command, commandDef));

                var paramDef = deviceDefinitionsManager.CommandPayloadDefinitions.Where(c => c.DeviceType == deviceConf.DeviceType && c.CommandType == command.CommandType && c.Command == command.Command).First();
                var payloadDict = CommandPayloadDictionary.GetOrAdd(deviceConf.DeviceId, new ConcurrentDictionary<string, object>());

                var (response, responseRaw) = await switchBotApiClient.GetDevicesAsync(CancellationToken.None);
                var deivce = response.DeviceList.Where(rd => rd!.DeviceId == deviceConf.DeviceId).FirstOrDefault();
                var keys = deivce!.KeyList.Select(key => $"{key.Id}:{key.Name}:{key.Type}").ToArray();
                selectEntities.Add(MqttEntityHelper.CreateKeypadDeleteKeySelectEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef, keys));
                payloadDict[paramDef.Name] = "";
                break;
            }

            switch (commandDef.PayloadType)
            {
                case PayloadType.SingleValue:
                case PayloadType.Json:
                case PayloadType.JoinColon:
                case PayloadType.JoinComma:
                case PayloadType.JoinSemiColon:
                    var deviceMqttForCommand = new DeviceMqtt(
                            identifiers: [$"{deviceConf.DeviceId}{command.Command}"],
                            name: $"{deviceConf.DeviceName}-{command.Command}",
                            manufacturer: deviceMqtt.Manufacturer,
                            model: deviceMqtt.Model);
                    commandDeviceEntities.Add(deviceMqttForCommand);
                    buttonEntities.Add(MqttEntityHelper.CreateCommandButtonEntity(deviceMqttForCommand, deviceConf, commandIndex, command, commandDef));

                    var paramDefs = deviceDefinitionsManager.CommandPayloadDefinitions.Where(c => c.DeviceType == deviceConf.DeviceType && c.CommandType == command.CommandType && c.Command == command.Command);
                    var payloadDict = CommandPayloadDictionary.GetOrAdd(deviceConf.DeviceId, new ConcurrentDictionary<string, object>());
                    foreach (var paramDef in paramDefs)
                    {
                        string? defaultValue = paramDef.DefaultValue;
                        switch (paramDef.ParameterType)
                        {
                            case ParameterType.Long:
                                defaultValue = paramDef.DefaultValue ?? "0";
                                numberEntities.Add(MqttEntityHelper.CreateCommandParamNumberEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef, paramDef.RangeMin - 1, paramDef.RangeMax, NumberMode.Box, defaultValue));
                                break;
                            case ParameterType.Range:
                                defaultValue = paramDef.DefaultValue ?? paramDef.RangeMin?.ToString() ?? string.Empty;
                                numberEntities.Add(MqttEntityHelper.CreateCommandParamNumberEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef, paramDef.RangeMin, paramDef.RangeMax, NumberMode.Slider, defaultValue));
                                break;
                            case ParameterType.Select:
                                defaultValue = paramDef.OptionToDescription(paramDef.DefaultValue);
                                selectEntities.Add(MqttEntityHelper.CreateCommandParamSelectEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef, defaultValue));
                                break;
                            case ParameterType.SelectOrRange:
                                defaultValue = paramDef.OptionToDescription(paramDef.DefaultValue);
                                selectEntities.Add(MqttEntityHelper.CreateCommandParamSelectEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef,
                                    defaultValue, "-S"));
                                numberEntities.Add(MqttEntityHelper.CreateCommandParamNumberEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef, paramDef.RangeMin, paramDef.RangeMax, NumberMode.Slider, null, "-N"));
                                break;
                            case ParameterType.String:
                                defaultValue = paramDef.DefaultValue ?? string.Empty;
                                textEntities.Add(MqttEntityHelper.CreateCommandParamTextEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef, defaultValue));
                                break;
                            default:
                                break;
                        }
                        payloadDict[paramDef.Name] = defaultValue ?? string.Empty;
                    }
                    break;
                case PayloadType.Default:
                default:
                    buttonEntities.Add(MqttEntityHelper.CreateCommandButtonEntity(deviceMqtt, deviceConf, commandIndex, command, commandDef));
                    break;
            }
        }

        //subscribe command actions
        mqttService.Subscribe(MqttEntityHelper.GetCommandTopic(deviceConf.DeviceId), async (payload) => await ReceiveCommandAsync(payload, deviceConf));

        foreach (var e in buttonEntities)
        {
            await PublishEntityAsync(e);
        }
        foreach (var e in numberEntities)
        {
            await PublishEntityAsync(e);
        }
        foreach (var e in selectEntities)
        {
            await PublishEntityAsync(e);
        }
        foreach (var e in textEntities)
        {
            await PublishEntityAsync(e);
        }
    }

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object>> CommandPayloadDictionary = new();

    private async Task ReceiveCommandAsync(string payloadRaw, DeviceBase device)
    {
        try
        {
            logger.LogTrace("receive command {DeviceId},{Payload}", device.DeviceId, payloadRaw);
            var payload = JsonSerializer.Deserialize<MqttCommandPayload>(payloadRaw)!;

            var commandType = payload.CommandType.ToEnumFromEnumMember<CommandType>()!;

            var commandConf = device.Commands.FirstOrDefault(c => c.CommandType == commandType && c.Command == payload.Command);
            var commandDef = deviceDefinitionsManager.CommandDefinitions.FirstOrDefault(c => c.DeviceType == device.DeviceType && c.CommandType == commandType && c.Command == payload.Command);

            if (commandConf == null)
            {
                logger.LogWarning("unknown command {DeviceId},{Payload}", device.DeviceId, payloadRaw);
                return;
            }

            // Keypad deleteKey
            if ((device.DeviceType == DeviceType.Keypad
                || device.DeviceType == DeviceType.KeypadTouch
                || device.DeviceType == DeviceType.KeypadVision)
                && payload.Command == "deleteKey")
            {
                if (payload.ParamValue == "reloadkeys")
                {
                    var paramDef = deviceDefinitionsManager.CommandPayloadDefinitions.Where(c => c.DeviceType == device.DeviceType && c.CommandType == commandType && c.Command == payload.Command).First();
                    var payloadDict = CommandPayloadDictionary.GetOrAdd(device.DeviceId, new ConcurrentDictionary<string, object>());

                    var (response, responseRaw) = await switchBotApiClient.GetDevicesAsync(CancellationToken.None);
                    var deivce = response.DeviceList.Where(rd => rd!.DeviceId == device.DeviceId).FirstOrDefault();
                    var keys = deivce!.KeyList.Select(key => $"{key.Id}:{key.Name}:{key.Type}").ToArray();
                    var commandIndex = device.Commands.Where(c => c.Enable).ToList().IndexOf(commandConf) + 1;

                    var deviceMqtt = new DeviceMqtt(
                        identifiers: [device.DeviceId],
                        name: device.DeviceName,
                        manufacturer: $"SwitchBot",
                        model: deviceDefinitionsManager.DeviceDefinitions.First(x => x.DeviceType == device.DeviceType).ApiDeviceTypeString);
                    var deviceMqttForCommand = new DeviceMqtt(
                            identifiers: [$"{device.DeviceId}{commandDef!.Command}"],
                            name: $"{device.DeviceName}-{commandDef.Command}",
                            manufacturer: deviceMqtt.Manufacturer,
                            model: deviceMqtt.Model);
                    await PublishEntityAsync(MqttEntityHelper.CreateKeypadDeleteKeySelectEntity(device, commandIndex, commandConf, deviceMqttForCommand, paramDef, keys));
                    payloadDict[paramDef.Name] = "";
                    return;
                }
                if (payload.ParamName == "id")
                {
                    CommandPayloadDictionary[device.DeviceId][payload.ParamName] = payload.ParamValue.Split(':').FirstOrDefault() ?? "";
                    return;
                }
            }

            if (payload.ParamName != MqttEntityHelper.ButtonPrefix)
            {
                CommandPayloadDictionary[device.DeviceId][payload.ParamName] = payload.ParamValue;
                return;
            }
            if (commandConf.CommandType == CommandType.Customize
                || commandConf.CommandType == CommandType.Tag
                || commandDef!.PayloadType == PayloadType.Default)
            {
                await switchBotApiClient.SendDefaultDeviceControlCommandAsync(device, commandConf, CancellationToken.None);
                return;
            }
            {

                var paramDefs = deviceDefinitionsManager.CommandPayloadDefinitions.Where(p => p.DeviceType == device.DeviceType && p.CommandType == commandType && p.Command == payload.Command).OrderBy(p => p.Index).ToList();
                var payloadDict = CommandPayloadDictionary.GetOrAdd(device.DeviceId, new ConcurrentDictionary<string, object>());
                if (commandDef.PayloadType == PayloadType.SingleValue)
                {
                    var value = payloadDict[paramDefs[0].Name];
                    var paramDef = paramDefs.Single();
                    if (paramDef.ParameterType == ParameterType.SelectOrRange)
                    {
                        if (long.TryParse((string)payloadDict[paramDef.Name], out var longValue))
                        {
                            value = longValue;
                        }
                        else
                        {
                            value = paramDef.DescriptionToOption((string)payloadDict[paramDef.Name]);
                        }
                    }
                    if (paramDef.ParameterType == ParameterType.Select)
                    {
                        value = paramDef.DescriptionToOption((string)payloadDict[paramDef.Name]);
                    }
                    if(device.DeviceType == DeviceType.RollerShade
                        && commandConf.Command == "setPosition"
                        && paramDef.ParameterType == ParameterType.Range)
                    {
                        value = int.Parse((string)value);
                    }
                    await switchBotApiClient.SendDeviceControlCommandAsync(device, commandConf, value, CancellationToken.None);
                    return;
                }

                if (commandDef.PayloadType == PayloadType.Json)
                {
                    JsonNode jsonRoot = new JsonObject();
                    paramDefs.ForEach(paramDef =>
                    {
                        var json = jsonRoot;
                        if (!string.IsNullOrEmpty(paramDef.Path))
                        {
                            if (jsonRoot[paramDef.Path] == null)
                            {
                                jsonRoot[paramDef.Path] = new JsonObject();
                            }
                            json = jsonRoot[paramDef.Path]!;
                        }
                        if (paramDef.ParameterType == ParameterType.Long
                            || paramDef.ParameterType == ParameterType.Range)
                        {
                            if (long.TryParse((string)payloadDict[paramDef.Name], out var longValue)
                                && longValue != paramDef.RangeMin - 1)
                            {
                                json[paramDef.Name] = JsonValue.Create<long>(longValue);
                            }
                        }
                        else if (paramDef.ParameterType == ParameterType.Select)
                        {
                            json[paramDef.Name] = paramDef.DescriptionToOption((string)payloadDict[paramDef.Name]);
                        }
                        else if (paramDef.ParameterType == ParameterType.SelectOrRange)
                        {
                            if (long.TryParse((string)payloadDict[paramDef.Name], out var longValue))
                            {
                                json[paramDef.Name] = JsonValue.Create<long>(longValue);
                            }
                            else
                            {
                                json[paramDef.Name] = paramDef.DescriptionToOption((string)payloadDict[paramDef.Name]);
                            }
                        }
                        else
                        {
                            json[paramDef.Name] = JsonValue.Create<string>((string)payloadDict[paramDef.Name]);
                        }
                    });
                    if ((device.DeviceType == DeviceType.AirPurifierPM25
                    || device.DeviceType == DeviceType.AirPurifierTablePM25
                    || device.DeviceType == DeviceType.AirPurifierVOC
                    || device.DeviceType == DeviceType.AirPurifierTableVOC)
                    && commandDef.Command == "setMode")
                    {
                        if (jsonRoot["mode"]!.AsValue().GetValue<string>() != "1")
                        {
                            jsonRoot.AsObject().Remove("fanGear");
                        }
                        jsonRoot["mode"] = JsonValue.Create<int>(int.Parse(jsonRoot["mode"]!.AsValue().GetValue<string>()));
                    }
                    await switchBotApiClient.SendDeviceControlCommandAsync(device, commandConf, jsonRoot, CancellationToken.None);
                }
                else
                {
                    var parameters = string.Join(
                        commandDef.PayloadType switch
                        {
                            PayloadType.JoinColon => ':',
                            PayloadType.JoinComma => ',',
                            PayloadType.JoinSemiColon => ';',
                            _ => throw new InvalidOperationException()
                        }
                        , paramDefs.Select(p =>
                        {
                            if (p.ParameterType == ParameterType.SelectOrRange)
                            {
                                if (long.TryParse((string)payloadDict[p.Name], out var longValue))
                                {
                                    return longValue;
                                }
                                else
                                {
                                    return p.DescriptionToOption((string)payloadDict[p.Name]);
                                }
                            }
                            if (p.ParameterType == ParameterType.Select)
                            {
                                return p.DescriptionToOption((string)payloadDict[p.Name]);
                            }
                            return payloadDict[p.Name];
                        }));
                    await switchBotApiClient.SendDeviceControlCommandAsync(device, commandConf, parameters, CancellationToken.None);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "subscribe action {DeviceId},{Payload}", device.DeviceId, payloadRaw);
        }
    }
    public async Task PublishWebhookAsync(JsonNode webhookContent, JsonNode inputRawRoot)
    {
        var deviceMac = webhookContent["deviceMac"]?.GetValue<string>();
        PhysicalDevice? physicalDevice = CurrentDevicesConfig.PhysicalDevices.FirstOrDefault(d => d.DeviceId == deviceMac);
        if (physicalDevice == null)
        {
            logger.LogWarning("unknown deviceMac {deviceMac}", deviceMac);
            return;
        }
        if (!physicalDevice.Enable || !physicalDevice.UseWebhook)
        {
            logger.LogInformation("disable webhook device {deviceId},{deviceName},{json}", physicalDevice.DeviceId, physicalDevice.DeviceId, webhookContent);
            return;
        }
        var webhook = await deviceStatePersistanceManager.LoadAsync(physicalDevice.DeviceId);
        var fieldDefs = deviceDefinitionsManager.FieldDefinitions.Where(f => f.DeviceType == physicalDevice.DeviceType);
        var contentKvDict = webhookContent.AsObject().ToDictionary();
        foreach (var rootKv in inputRawRoot.AsObject())
        {
            if (
                rootKv.Key != "eventType"
                && rootKv.Key != "eventVersion"
                && rootKv.Key != "context"
                && !contentKvDict.ContainsKey(rootKv.Key))
            {
                contentKvDict.Add(rootKv.Key, rootKv.Value);
            }
        }
        foreach (var kv in contentKvDict)
        {
            var fieldDef = fieldDefs.FirstOrDefault(f => f.WebhookKey == kv.Key);
            if (fieldDef == null)
            {
                logger.LogWarning("unknown webhook paylod {deviceType},{key},{value}", physicalDevice.DeviceType, kv.Key, kv.Value?.ToJsonString());
                continue;
            }

            var field = physicalDevice.Fields.First(f => f.FieldName == fieldDef.FieldName);
            if (!field.Enable)
            {
                logger.LogTrace("disable webhook paylod {deviceType},{key},{value}", physicalDevice.DeviceType, kv.Key, kv.Value?.ToJsonString());
                continue;
            }

            if (kv.Key == "deviceType") //modify device name
            {
                if (physicalDevice.DeviceType == DeviceType.BatteryCirculatorFan
                    || physicalDevice.DeviceType == DeviceType.CirculatorFan)
                {
                    webhook[fieldDef.FieldName] = deviceDefinitionsManager.DeviceDefinitions.First(x => x.DeviceType == physicalDevice.DeviceType).ApiDeviceTypeString;
                }
                else
                {
                    webhook[fieldDef.FieldName] = deviceDefinitionsManager.DeviceDefinitions.FirstOrDefault(x => x.WebhookDeviceTypeString == kv.Value!.GetValue<string>())?.ApiDeviceTypeString
                        ?? deviceDefinitionsManager.DeviceDefinitions.First(x => x.DeviceType == physicalDevice.DeviceType).ApiDeviceTypeString;
                }
            }
            else
            {
                webhook[fieldDef.FieldName] = kv.Value!.Copy();
            }
            if (
                (
                    (fieldDef.DeviceType == DeviceType.Lock
                    || fieldDef.DeviceType == DeviceType.LockPro
                    || fieldDef.DeviceType == DeviceType.LockLite
                    || fieldDef.DeviceType == DeviceType.LockUltra)
                    && fieldDef.FieldName == "lockState")
                ||
                (
                    (fieldDef.DeviceType == DeviceType.PlugMiniJp
                    || fieldDef.DeviceType == DeviceType.PlugMiniUs)
                    && fieldDef.FieldName == "powerState")
                ||
                (
                    (fieldDef.DeviceType == DeviceType.CeilingLight
                    || fieldDef.DeviceType == DeviceType.CeilingLightPro
                    || fieldDef.DeviceType == DeviceType.StripLight
                    || fieldDef.DeviceType == DeviceType.ColorBulb
                    || fieldDef.DeviceType == DeviceType.StripLight3
                    || fieldDef.DeviceType == DeviceType.FloorLamp
                    || fieldDef.DeviceType == DeviceType.BatteryCirculatorFan
                    || fieldDef.DeviceType == DeviceType.CirculatorFan
                    || fieldDef.DeviceType == DeviceType.EvaporativeHumidifier
                    || fieldDef.DeviceType == DeviceType.AirPurifierPM25
                    || fieldDef.DeviceType == DeviceType.AirPurifierTablePM25
                    || fieldDef.DeviceType == DeviceType.AirPurifierVOC
                    || fieldDef.DeviceType == DeviceType.AirPurifierTableVOC)
                    && fieldDef.FieldName == "power")
              )
            {
                webhook[fieldDef.FieldName] = webhook[fieldDef.FieldName]!.GetValue<string>().ToLower();
            }
        }
        webhook["webhook_timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await mqttService.PublishAsync(MqttEntityHelper.GetStateTopic(physicalDevice.DeviceId), JsonSerializer.Serialize(webhook), messageRetailOptions.Value.State);
        await deviceStatePersistanceManager.SaveAsync(physicalDevice.DeviceId, webhook);
    }

    public async Task PollingAndPublishStatusAsync(PhysicalDevice physicalDevice, CancellationToken cancellationToken)
    {
        try
        {
            var rawStatus = await switchBotApiClient.GetDeviceStatus(physicalDevice.DeviceId, cancellationToken);
            var status = await deviceStatePersistanceManager.LoadAsync(physicalDevice.DeviceId);

            var fieldDefs = deviceDefinitionsManager.FieldDefinitions.Where(f => f.DeviceType == physicalDevice.DeviceType);

            List<KeyValuePair<string, JsonNode?>> keyValuePairs = [];
            foreach (var kv in rawStatus.AsObject())
            {
                if (kv.Value?.GetValueKind() == JsonValueKind.Object)
                {
                    // EvaporativeHumidifier
                    //  filterElement.effectiveUsageHours
                    //  filterElement.usedHours
                    foreach (var nestKv in kv.Value!.AsObject())
                    {
                        keyValuePairs.Add(new KeyValuePair<string, JsonNode?>($"{kv.Key}.{nestKv.Key}", nestKv.Value));
                    }
                }
                else
                {
                    keyValuePairs.Add(kv);
                }
            }

            foreach (var kv in keyValuePairs)
            {
                var fieldDef = fieldDefs.FirstOrDefault(f => f.StatusKey == kv.Key);
                if (fieldDef == null)
                {
                    logger.LogWarning("unknown status paylod {deviceType},{key},{value}", physicalDevice.DeviceType, kv.Key, kv.Value?.ToJsonString());
                    continue;
                }

                var field = physicalDevice.Fields.FirstOrDefault(f => f.FieldName == fieldDef.FieldName);
                if (field == null)
                {
                    logger.LogWarning("missing device field difinition(might need to remove and refetch the device) {deviceType},{key},{value}", physicalDevice.DeviceType, kv.Key, kv.Value?.ToJsonString());
                    continue;
                }
                if (!field.Enable)
                {
                    logger.LogTrace("disable polling paylod {deviceType},{key},{value}", physicalDevice.DeviceType, kv.Key, kv.Value?.ToJsonString());
                    continue;
                }
                status[fieldDef.FieldName] = kv.Value!.Copy();
                if (
                    (
                        (fieldDef.DeviceType == DeviceType.EvaporativeHumidifier
                        || fieldDef.DeviceType == DeviceType.AirPurifierPM25
                        || fieldDef.DeviceType == DeviceType.AirPurifierTablePM25
                        || fieldDef.DeviceType == DeviceType.AirPurifierVOC
                        || fieldDef.DeviceType == DeviceType.AirPurifierTableVOC)
                        && fieldDef.FieldName == "power")
                  )
                {
                    status[fieldDef.FieldName] = status[fieldDef.FieldName]!.GetValue<string>().ToLower();
                }
            }
            status["status_timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            await mqttService.PublishAsync(MqttEntityHelper.GetStateTopic(physicalDevice.DeviceId), JsonSerializer.Serialize(status), messageRetailOptions.Value.State);
            await deviceStatePersistanceManager.SaveAsync(physicalDevice.DeviceId, status);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "polling {deviceType},{deviceId}", physicalDevice.DeviceType, physicalDevice.DeviceId);
        }
    }

    private async Task PublishSceneEntities(CancellationToken cancellationToken)
    {
        var sceneList = await switchBotApiClient.GetSceneList(cancellationToken);
        foreach (var scene in sceneList)
        {
            //subscribe update action
            await PublishEntityAsync(MqttEntityHelper.CreateSceneEntity(scene.SceneName, scene.SceneId));
        }
        mqttService.Subscribe(MqttEntityHelper.GetSceneCommandTopic(), async (payload) =>
        {
            try
            {
                await switchBotApiClient.ExecuteManualSceneAsync(payload, CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{payload}", payload);
            }
        }
        );
    }
}
public static class JsonNodeExtensions
{
    public static T Copy<T>(this T node) where T : JsonNode => node.Deserialize<T>()!;
}