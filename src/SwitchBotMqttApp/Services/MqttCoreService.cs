using HomeAssistantAddOn.Mqtt;
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

/// <summary>
/// Core MQTT service that manages the bridge between SwitchBot devices and MQTT broker.
/// Handles device state publishing, command subscription, and Home Assistant MQTT Discovery integration.
/// </summary>
public class MqttCoreService(
    ILogger<MqttCoreService> logger
        , DeviceConfigurationManager deviceConfigurationManager
        , DeviceDefinitionsManager deviceDefinitionsManager
        , MqttService mqttService
        , SwitchBotApiClient switchBotApiClient
        , IOptions<MessageRetainOptions> messageRetailOptions
        , DeviceStatePersistanceManager deviceStatePersistanceManager) : ManagedServiceBase
{
    /// <summary>
    /// Gets or sets the current device configuration including physical and virtual IR remote devices.
    /// </summary>
    public DevicesConfig CurrentDevicesConfig { get; set; } = default!;

    /// <summary>
    /// Starts the MQTT core service, initializes MQTT connection, and publishes device entities to Home Assistant.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Status = ServiceStatus.Starting;
            // Load device configuration from file
            CurrentDevicesConfig = await deviceConfigurationManager.GetFileAsync(cancellationToken);
            CommandPayloadDictionary.Clear();

            // Start MQTT client connection
            await mqttService.StartAsync();

            // Process both physical devices and virtual IR remote devices that are enabled
            foreach (var deviceConf in CurrentDevicesConfig.PhysicalDevices.Where(d => d.Enable).Select(d => (DeviceBase)d)
                                            .Union(CurrentDevicesConfig.VirtualInfraredRemoteDevices.Where(d => d.Enable)))
            {
                var deviceDef = deviceDefinitionsManager.DeviceDefinitions.First(d => d.DeviceType == deviceConf.DeviceType);
                // Create Home Assistant device entry
                var deviceMqtt = new DeviceMqtt(
                    identifiers: [deviceConf.DeviceId],
                    name: deviceConf.DeviceName,
                    manufacturer: $"SwitchBot{(deviceConf is VirtualInfraredRemoteDevice ? "(VirtualInfraredRemoteDevice)" : "")}",
                    model: deviceDef.ApiDeviceTypeString);


                // Publish field entities (sensors) for physical devices
                if (deviceConf is PhysicalDevice physicalDevice
                    && physicalDevice.Fields.Any(s => s.Enable))
                {
                    await PublishFieldEntities(deviceDef, deviceMqtt, physicalDevice, cancellationToken);

                }
                
                // Publish command entities (buttons, numbers, selects) for device control
                await PublishCommandEntities(deviceMqtt, deviceConf);
            }

            // Publish manual scene entities as buttons
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

    /// <summary>
    /// Stops the MQTT core service and disconnects from the MQTT broker.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await mqttService.StopAsync();
        logger.LogInformation("stopped");
        Status = ServiceStatus.Stoped;
        CommandPayloadDictionary.Clear();
    }

    /// <summary>
    /// Publishes field entities (sensors and binary sensors) for a physical device to Home Assistant via MQTT Discovery.
    /// </summary>
    /// <param name="deviceDef">Device definition containing field metadata.</param>
    /// <param name="deviceMqtt">MQTT device information for Home Assistant integration.</param>
    /// <param name="physicalDevice">Physical device configuration.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task PublishFieldEntities(DeviceDefinition deviceDef, DeviceMqtt deviceMqtt, PhysicalDevice physicalDevice, CancellationToken cancellationToken)
    {
        var fieldDefs = deviceDef.Fields;
        // Create sensor/binary_sensor entities for each enabled field
        var statusEntities = physicalDevice.Fields.Where(s => s.Enable)
            .Select(s =>
            {
                var fieldDef = fieldDefs.Where(f => f.FieldName == s.FieldName).First();
                // Binary fields become binary_sensor entities (on/off)
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
                    // Non-binary fields become sensor entities
                    string? value_template = null;
                    // Special handling for Unix timestamp conversion
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

        // Add timestamp sensor for status polling updates
        var statusTimestamp = new SensorConfig(deviceMqtt
            , key: "status_timestamp"
            , name: "Status Lastupdate"
            , objectId: $"{FieldSourceType.Status.ToEnumMemberValue()!}_status_timestamp_{deviceMqtt.Identifiers[0]}"
            , stateTopic: MqttEntityHelper.GetStateTopic(deviceMqtt.Identifiers[0])
            , deviceClass: SensorDeviceClass.Timestamp
            , value_template: UnixTimeValueTemplateFormat.Replace("%FIELD%", "status_timestamp")
            );
        await PublishEntityAsync(statusTimestamp);

        // Add timestamp sensor for webhook updates if webhook is enabled
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

        // Create manual update button and subscribe to its command topic
        await PublishEntityAsync(MqttEntityHelper.CreateSensorUpdateButtonEntity(deviceMqtt, physicalDevice));
        mqttService.Subscribe(MqttEntityHelper.GetSensorUpdateTopic(physicalDevice.DeviceId), async (payload) => await PollingAndPublishStatusAsync(physicalDevice, CancellationToken.None));

        // Fetch and publish initial device status
        await PollingAndPublishStatusAsync(physicalDevice, cancellationToken);
    }

    /// <summary>
    /// Value template format for converting Unix timestamps to local datetime in Home Assistant.
    /// </summary>
    private const string UnixTimeValueTemplateFormat = "{% set ts = value_json.get('%FIELD%', {})  %} {% if ts %}\n  {{ (ts / 1000) | timestamp_local | as_datetime }}\n{% else %}\n  {{ this.state }}\n{% endif %}";

    /// <summary>
    /// Publishes an MQTT entity configuration to Home Assistant Discovery topic.
    /// </summary>
    /// <param name="payload">MQTT entity configuration payload.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task PublishEntityAsync(MqttEntityBase payload)
    {
        await mqttService.PublishAsync(payload.Topic, payload, messageRetailOptions.Value.Entity);
    }

    /// <summary>
    /// Publishes command entities (buttons, numbers, selects, texts) for device control to Home Assistant.
    /// </summary>
    /// <param name="deviceMqtt">MQTT device information for Home Assistant integration.</param>
    /// <param name="deviceConf">Device configuration containing enabled commands.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
            // Use predefined command definition for standard commands
            if (command.CommandType == CommandType.Command)
            {
                commandDef = deviceDefinitionsManager.DeviceDefinitions.First(d => d.DeviceType == deviceConf.DeviceType).Commands.First(c => c.CommandType == command.CommandType && c.Command == command.Command);
            }
            else
            {
                // Create command definition for customized or tagged commands
                commandDef = new CommandDefinition()
                {
                    CommandType = command.CommandType,
                    Command = command.Command,
                    ButtonDeviceClass = ButtonDeviceClass.None,
                    Description = command.DisplayName,
                    DisplayName = command.DisplayName,
                    Icon = null,
                    PayloadType = PayloadType.Default,
                };
            }

            // Special handling for Keypad deleteKey command - requires loading keys from API
            if ((deviceConf.DeviceType == DeviceType.Keypad
                || deviceConf.DeviceType == DeviceType.KeypadTouch
                || deviceConf.DeviceType == DeviceType.KeypadVision
                || deviceConf.DeviceType == DeviceType.KeypadVisionPro)
                && command.Command == "deleteKey")
            {
                // Create separate device for deleteKey command
                var deviceMqttForCommand = new DeviceMqtt(
                        identifiers: [$"{deviceConf.DeviceId}{command.Command}"],
                        name: $"{deviceConf.DeviceName}-{command.Command}",
                        manufacturer: deviceMqtt.Manufacturer,
                        model: deviceMqtt.Model);
                commandDeviceEntities.Add(deviceMqttForCommand);
                buttonEntities.Add(MqttEntityHelper.CreateCommandButtonEntity(deviceMqttForCommand, deviceConf, commandIndex, command, commandDef));
                buttonEntities.Add(MqttEntityHelper.CreateKeypadReloadButtonEntity(deviceMqttForCommand, deviceConf, commandIndex, command, commandDef));

                var paramDef = commandDef.Payloads.First(c => c.CommandType == command.CommandType && c.Command == command.Command);
                var payloadDict = CommandPayloadDictionary.GetOrAdd(deviceConf.DeviceId, new ConcurrentDictionary<string, object>());

                // Fetch keypad keys from API
                var (response, responseRaw) = await switchBotApiClient.GetDevicesAsync(CancellationToken.None);
                var deivce = response.DeviceList.Where(rd => rd!.DeviceId == deviceConf.DeviceId).FirstOrDefault();
                var keys = deivce!.KeyList.Select(key => $"{key.Id}:{key.Name}:{key.Type}").ToArray();
                selectEntities.Add(MqttEntityHelper.CreateKeypadDeleteKeySelectEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef, keys));
                payloadDict[paramDef.Name] = "";
                break;
            }

            switch (commandDef.PayloadType)
            {
                // Commands with parameters need separate device and parameter entities
                case PayloadType.SingleValue:
                case PayloadType.Json:
                case PayloadType.JoinColon:
                case PayloadType.JoinComma:
                case PayloadType.JoinSemiColon:
                    // Create separate device for parameterized commands
                    var deviceMqttForCommand = new DeviceMqtt(
                            identifiers: [$"{deviceConf.DeviceId}{command.Command}"],
                            name: $"{deviceConf.DeviceName}-{command.Command}",
                            manufacturer: deviceMqtt.Manufacturer,
                            model: deviceMqtt.Model);
                    commandDeviceEntities.Add(deviceMqttForCommand);
                    buttonEntities.Add(MqttEntityHelper.CreateCommandButtonEntity(deviceMqttForCommand, deviceConf, commandIndex, command, commandDef));

                    var paramDefs = commandDef.Payloads.Where(c => c.CommandType == command.CommandType && c.Command == command.Command);
                    var payloadDict = CommandPayloadDictionary.GetOrAdd(deviceConf.DeviceId, new ConcurrentDictionary<string, object>());
                    // Create parameter entities based on parameter type
                    foreach (var paramDef in paramDefs)
                    {
                        string? defaultValue = paramDef.DefaultValue;
                        switch (paramDef.ParameterType)
                        {
                            case ParameterType.Long:
                                // Long values use number entity with box mode
                                defaultValue = paramDef.DefaultValue ?? "0";
                                numberEntities.Add(MqttEntityHelper.CreateCommandParamNumberEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef, paramDef.RangeMin - 1, paramDef.RangeMax, NumberMode.Box, defaultValue));
                                break;
                            case ParameterType.Range:
                                // Range values use number entity with slider mode
                                defaultValue = paramDef.DefaultValue ?? paramDef.RangeMin?.ToString() ?? string.Empty;
                                numberEntities.Add(MqttEntityHelper.CreateCommandParamNumberEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef, paramDef.RangeMin, paramDef.RangeMax, NumberMode.Slider, defaultValue));
                                break;
                            case ParameterType.Select:
                                // Select values use dropdown select entity
                                defaultValue = paramDef.OptionToDescription(paramDef.DefaultValue);
                                selectEntities.Add(MqttEntityHelper.CreateCommandParamSelectEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef, defaultValue));
                                break;
                            case ParameterType.SelectOrRange:
                                // SelectOrRange creates both select and number entities
                                defaultValue = paramDef.OptionToDescription(paramDef.DefaultValue);
                                selectEntities.Add(MqttEntityHelper.CreateCommandParamSelectEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef,
                                    defaultValue, "-S"));
                                numberEntities.Add(MqttEntityHelper.CreateCommandParamNumberEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef, paramDef.RangeMin, paramDef.RangeMax, NumberMode.Slider, null, "-N"));
                                break;
                            case ParameterType.String:
                                // String values use text input entity
                                defaultValue = paramDef.DefaultValue ?? string.Empty;
                                textEntities.Add(MqttEntityHelper.CreateCommandParamTextEntity(deviceConf, commandIndex, command, deviceMqttForCommand, paramDef, defaultValue));
                                break;
                            default:
                                break;
                        }
                        // Initialize payload dictionary with default value
                        payloadDict[paramDef.Name] = defaultValue ?? string.Empty;
                    }
                    break;
                case PayloadType.Default:
                default:
                    // Simple commands just need a button entity
                    buttonEntities.Add(MqttEntityHelper.CreateCommandButtonEntity(deviceMqtt, deviceConf, commandIndex, command, commandDef));
                    break;
            }
        }

        // Subscribe to command topic to receive commands from Home Assistant
        mqttService.Subscribe(MqttEntityHelper.GetCommandTopic(deviceConf.DeviceId), async (payload) => await ReceiveCommandAsync(payload, deviceConf));

        // Publish all created entities
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

    /// <summary>
    /// Dictionary storing command payload parameters for each device.
    /// Key: DeviceId, Value: Dictionary of parameter names and their current values.
    /// </summary>
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object>> CommandPayloadDictionary = new();

    /// <summary>
    /// Receives and processes commands from MQTT for device control.
    /// Handles various command types including default, JSON, and parameterized commands.
    /// </summary>
    /// <param name="payloadRaw">Raw MQTT command payload as JSON string.</param>
    /// <param name="device">Target device for the command.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ReceiveCommandAsync(string payloadRaw, DeviceBase device)
    {
        try
        {
            logger.LogTrace("receive command {DeviceId},{Payload}", device.DeviceId, payloadRaw);
            var payload = JsonSerializer.Deserialize<MqttCommandPayload>(payloadRaw)!;

            var commandType = payload.CommandType.ToEnumFromEnumMember<CommandType>()!;

            var commandConf = device.Commands.FirstOrDefault(c => c.CommandType == commandType && c.Command == payload.Command);
            var commandDef = deviceDefinitionsManager.DeviceDefinitions.First(d => d.DeviceType == device.DeviceType).Commands.FirstOrDefault(c => c.CommandType == commandType && c.Command == payload.Command);

            if (commandConf == null)
            {
                logger.LogWarning("unknown command {DeviceId},{Payload}", device.DeviceId, payloadRaw);
                return;
            }

            // Special handling for Keypad deleteKey command
            if ((device.DeviceType == DeviceType.Keypad
                || device.DeviceType == DeviceType.KeypadTouch
                || device.DeviceType == DeviceType.KeypadVision
                || device.DeviceType == DeviceType.KeypadVisionPro)
                && payload.Command == "deleteKey")
            {
                // Handle reload keys request
                if (payload.ParamValue == "reloadkeys")
                {
                    var paramDef = commandDef!.Payloads.First(c => c.CommandType == commandType && c.Command == payload.Command);
                    var payloadDict = CommandPayloadDictionary.GetOrAdd(device.DeviceId, new ConcurrentDictionary<string, object>());

                    // Fetch updated key list from API
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
                    // Re-publish select entity with updated key list
                    await PublishEntityAsync(MqttEntityHelper.CreateKeypadDeleteKeySelectEntity(device, commandIndex, commandConf, deviceMqttForCommand, paramDef, keys));
                    payloadDict[paramDef.Name] = "";
                    return;
                }
                // Extract key ID from composite value (format: "id:name:type")
                if (payload.ParamName == "id")
                {
                    CommandPayloadDictionary[device.DeviceId][payload.ParamName] = payload.ParamValue.Split(':').FirstOrDefault() ?? "";
                    return;
                }
            }

            // Update parameter value in dictionary (not a button press)
            if (payload.ParamName != MqttEntityHelper.ButtonPrefix)
            {
                CommandPayloadDictionary[device.DeviceId][payload.ParamName] = payload.ParamValue;
                return;
            }
            
            // Execute command for Customize/Tag commands or commands with default payload
            if (commandConf.CommandType == CommandType.Customize
                || commandConf.CommandType == CommandType.Tag
                || commandDef!.PayloadType == PayloadType.Default)
            {
                await switchBotApiClient.SendDefaultDeviceControlCommandAsync(device, commandConf, CancellationToken.None);
                return;
            }
            {
                // Build command payload from stored parameter values
                var paramDefs = commandDef.Payloads.Where(c => c.CommandType == commandType && c.Command == payload.Command).OrderBy(p => p.Index).ToList();
                var payloadDict = CommandPayloadDictionary.GetOrAdd(device.DeviceId, new ConcurrentDictionary<string, object>());
                
                // Single value payload - send single parameter
                if (commandDef.PayloadType == PayloadType.SingleValue)
                {
                    var value = payloadDict[paramDefs[0].Name];
                    var paramDef = paramDefs.Single();
                    // Handle SelectOrRange: try parse as number, fallback to option value
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
                    // Convert description to option value for Select
                    if (paramDef.ParameterType == ParameterType.Select)
                    {
                        value = paramDef.DescriptionToOption((string)payloadDict[paramDef.Name]);
                    }
                    // RollerShade setPosition requires integer value
                    if (device.DeviceType == DeviceType.RollerShade
                        && commandConf.Command == "setPosition"
                        && paramDef.ParameterType == ParameterType.Range)
                    {
                        value = int.Parse((string)value);
                    }
                    await switchBotApiClient.SendDeviceControlCommandAsync(device, commandConf, value, CancellationToken.None);
                    return;
                }

                // JSON payload - build nested JSON structure
                if (commandDef.PayloadType == PayloadType.Json)
                {
                    JsonNode jsonRoot = new JsonObject();
                    paramDefs.ForEach(paramDef =>
                    {
                        var json = jsonRoot;
                        // Handle nested JSON paths
                        if (!string.IsNullOrEmpty(paramDef.Path))
                        {
                            if (jsonRoot[paramDef.Path] == null)
                            {
                                jsonRoot[paramDef.Path] = new JsonObject();
                            }
                            json = jsonRoot[paramDef.Path]!;
                        }
                        // Add parameter to JSON based on type
                        if (paramDef.ParameterType == ParameterType.Long
                            || paramDef.ParameterType == ParameterType.Range)
                        {
                            // Only include if value is valid (not minimum-1)
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
                    // Special handling for Air Purifier setMode command
                    if ((device.DeviceType == DeviceType.AirPurifierPM25
                    || device.DeviceType == DeviceType.AirPurifierTablePM25
                    || device.DeviceType == DeviceType.AirPurifierVOC
                    || device.DeviceType == DeviceType.AirPurifierTableVOC)
                    && commandDef.Command == "setMode")
                    {
                        // Remove fanGear if not in manual mode (mode != "1")
                        if (jsonRoot["mode"]!.AsValue().GetValue<string>() != "1")
                        {
                            jsonRoot.AsObject().Remove("fanGear");
                        }
                        // Convert mode from string to int
                        jsonRoot["mode"] = JsonValue.Create<int>(int.Parse(jsonRoot["mode"]!.AsValue().GetValue<string>()));
                    }
                    await switchBotApiClient.SendDeviceControlCommandAsync(device, commandConf, jsonRoot, CancellationToken.None);
                }
                else
                {
                    // Join payload - concatenate parameters with delimiter
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

    /// <summary>
    /// Publishes webhook event data to MQTT state topic for a physical device.
    /// Processes webhook payload and updates device state in Home Assistant.
    /// </summary>
    /// <param name="webhookContent">Webhook content from SwitchBot Cloud.</param>
    /// <param name="inputRawRoot">Raw webhook input root node.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
        
        // Load existing device state
        var webhook = await deviceStatePersistanceManager.LoadAsync(physicalDevice.DeviceId);
        var fieldDefs = deviceDefinitionsManager.DeviceDefinitions.First(f => f.DeviceType == physicalDevice.DeviceType).Fields;
        
        // Merge webhook content with root-level properties
        var contentKvDict = webhookContent.AsObject().ToDictionary();
        foreach (var rootKv in inputRawRoot.AsObject())
        {
            // Skip meta fields and already included fields
            if (
                rootKv.Key != "eventType"
                && rootKv.Key != "eventVersion"
                && rootKv.Key != "context"
                && !contentKvDict.ContainsKey(rootKv.Key))
            {
                contentKvDict.Add(rootKv.Key, rootKv.Value);
            }
        }
        
        // Process each webhook field
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

            // Special handling for deviceType field
            if (kv.Key == "deviceType")
            {
                // CirculatorFan devices report incorrect device type in webhook
                if (physicalDevice.DeviceType == DeviceType.BatteryCirculatorFan
                    || physicalDevice.DeviceType == DeviceType.CirculatorFan)
                {
                    webhook[fieldDef.FieldName] = deviceDefinitionsManager.DeviceDefinitions.First(x => x.DeviceType == physicalDevice.DeviceType).ApiDeviceTypeString;
                }
                else
                {
                    // Map webhook device type to API device type
                    webhook[fieldDef.FieldName] = deviceDefinitionsManager.DeviceDefinitions.FirstOrDefault(x => x.WebhookDeviceTypeString == kv.Value!.GetValue<string>())?.ApiDeviceTypeString
                        ?? deviceDefinitionsManager.DeviceDefinitions.First(x => x.DeviceType == physicalDevice.DeviceType).ApiDeviceTypeString;
                }
            }
            else
            {
                webhook[fieldDef.FieldName] = kv.Value!.Copy();
            }
            
            // Normalize lockState value for Lock devices
            if (
                (
                    (physicalDevice.DeviceType == DeviceType.Lock
                    || physicalDevice.DeviceType == DeviceType.LockPro
                    || physicalDevice.DeviceType == DeviceType.LockLite
                    || physicalDevice.DeviceType == DeviceType.LockUltra)
                    && fieldDef.FieldName == "lockState")
              )
            {
                var val = webhook[fieldDef.FieldName]!.GetValue<string>().ToLower();
                // Treat "latchboltlocked" as "unlocked"
                if (val == "latchboltlocked")
                {
                    val = "unlocked";
                }
                webhook[fieldDef.FieldName] = val;
            }
            
            // Normalize power state to lowercase for consistency
            if (
                (
                    (physicalDevice.DeviceType == DeviceType.PlugMiniJp
                    || physicalDevice.DeviceType == DeviceType.PlugMiniUs
                    || physicalDevice.DeviceType == DeviceType.PlugMiniEu)
                    && fieldDef.FieldName == "powerState")
                ||
                (
                    (physicalDevice.DeviceType == DeviceType.CeilingLight
                    || physicalDevice.DeviceType == DeviceType.CeilingLightPro
                    || physicalDevice.DeviceType == DeviceType.StripLight
                    || physicalDevice.DeviceType == DeviceType.ColorBulb
                    || physicalDevice.DeviceType == DeviceType.StripLight3
                    || physicalDevice.DeviceType == DeviceType.FloorLamp
                    || physicalDevice.DeviceType == DeviceType.RGBICWWStripLight
                    || physicalDevice.DeviceType == DeviceType.RGBICWWFloorLamp
                    || physicalDevice.DeviceType == DeviceType.RGBICNeonWireRopeLight
                    || physicalDevice.DeviceType == DeviceType.RGBICNeonRopeLight
                    || physicalDevice.DeviceType == DeviceType.CandleWarmerLamp
                    || physicalDevice.DeviceType == DeviceType.BatteryCirculatorFan
                    || physicalDevice.DeviceType == DeviceType.CirculatorFan
                    || physicalDevice.DeviceType == DeviceType.EvaporativeHumidifier
                    || physicalDevice.DeviceType == DeviceType.AirPurifierPM25
                    || physicalDevice.DeviceType == DeviceType.AirPurifierTablePM25
                    || physicalDevice.DeviceType == DeviceType.AirPurifierVOC
                    || physicalDevice.DeviceType == DeviceType.AirPurifierTableVOC)
                    && fieldDef.FieldName == "power")
              )
            {
                webhook[fieldDef.FieldName] = webhook[fieldDef.FieldName]!.GetValue<string>().ToLower();
            }
        }
        
        // Add webhook timestamp
        webhook["webhook_timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        
        // Publish updated state to MQTT and persist to file
        await mqttService.PublishAsync(MqttEntityHelper.GetStateTopic(physicalDevice.DeviceId), JsonSerializer.Serialize(webhook), messageRetailOptions.Value.State);
        await deviceStatePersistanceManager.SaveAsync(physicalDevice.DeviceId, webhook);
    }

    /// <summary>
    /// Polls device status from SwitchBot API and publishes updated state to MQTT.
    /// </summary>
    /// <param name="physicalDevice">Physical device to poll and update.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PollingAndPublishStatusAsync(PhysicalDevice physicalDevice, CancellationToken cancellationToken)
    {
        try
        {
            // Fetch current status from SwitchBot API
            var rawStatus = await switchBotApiClient.GetDeviceStatus(physicalDevice.DeviceId, cancellationToken);
            var status = await deviceStatePersistanceManager.LoadAsync(physicalDevice.DeviceId);

            var fieldDefs = deviceDefinitionsManager.DeviceDefinitions.First(f => f.DeviceType == physicalDevice.DeviceType).Fields;

            // Flatten nested objects (e.g., EvaporativeHumidifier has filterElement.effectiveUsageHours)
            List<KeyValuePair<string, JsonNode?>> keyValuePairs = [];
            foreach (var kv in rawStatus.AsObject())
            {
                if (kv.Value?.GetValueKind() == JsonValueKind.Object)
                {
                    // Flatten nested properties with dot notation
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

            // Process each status field
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
                
                // Normalize power state to lowercase for specific devices
                if (
                    (
                        (physicalDevice.DeviceType == DeviceType.EvaporativeHumidifier
                        || physicalDevice.DeviceType == DeviceType.AirPurifierPM25
                        || physicalDevice.DeviceType == DeviceType.AirPurifierTablePM25
                        || physicalDevice.DeviceType == DeviceType.AirPurifierVOC
                        || physicalDevice.DeviceType == DeviceType.AirPurifierTableVOC)
                        && fieldDef.FieldName == "power")
                  )
                {
                    status[fieldDef.FieldName] = status[fieldDef.FieldName]!.GetValue<string>().ToLower();
                }
                
                // Normalize lockState value for Lock devices
                if (
                    (
                        (physicalDevice.DeviceType == DeviceType.Lock
                        || physicalDevice.DeviceType == DeviceType.LockPro
                        || physicalDevice.DeviceType == DeviceType.LockLite
                        || physicalDevice.DeviceType == DeviceType.LockUltra)
                        && fieldDef.FieldName == "lockState")
                  )
                {
                    var val = status[fieldDef.FieldName]!.GetValue<string>().ToLower();
                    // Treat "latchboltlocked" as "unlocked"
                    if (val == "latchboltlocked")
                    {
                        val = "unlocked";
                    }
                    status[fieldDef.FieldName] = val;
                }
            }
            
            // Add status polling timestamp
            status["status_timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            
            // Publish updated state to MQTT and persist to file
            await mqttService.PublishAsync(MqttEntityHelper.GetStateTopic(physicalDevice.DeviceId), JsonSerializer.Serialize(status), messageRetailOptions.Value.State);
            await deviceStatePersistanceManager.SaveAsync(physicalDevice.DeviceId, status);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "polling {deviceType},{deviceId}", physicalDevice.DeviceType, physicalDevice.DeviceId);
        }
    }

    /// <summary>
    /// Publishes scene entities as button controls to Home Assistant for manual scene execution.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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

/// <summary>
/// Extension methods for <see cref="JsonNode"/> operations.
/// </summary>
public static class JsonNodeExtensions
{
    /// <summary>
    /// Creates a deep copy of a JSON node.
    /// </summary>
    /// <typeparam name="T">Type of JSON node to copy.</typeparam>
    /// <param name="node">Source JSON node.</param>
    /// <returns>A deep copy of the JSON node.</returns>
    public static T Copy<T>(this T node) where T : JsonNode => node.Deserialize<T>()!;
}