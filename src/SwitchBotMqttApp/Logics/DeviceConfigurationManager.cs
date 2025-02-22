﻿using HomeAssistantAddOn.Core;
using Microsoft.Extensions.Options;
using SwitchBotMqttApp.Configurations;
using SwitchBotMqttApp.Models.DeviceConfiguration;
using System.Text;
using System.Text.Json;

namespace SwitchBotMqttApp.Logics;

public class DeviceConfigurationManager(
    ILogger<DeviceConfigurationManager> logger
        , DeviceDefinitionsManager deviceDefinitionsManager
        , SwitchBotApiClient switchBotApiClient
        , IOptions<EnforceDeviceTypeOptions> enforceDeviceTypeOptions)
{
    private static readonly string DeviceConfigurationFilePath = Path.Combine(Utility.GetBaseDataDirectory(), "switchbot_config.json");
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
    };

    #region File I/O
    public async Task<DevicesConfig> GetFileAsync(CancellationToken cancellationToken = default)
    {
        if (File.Exists(DeviceConfigurationFilePath))
        {
            var json = await File.ReadAllTextAsync(DeviceConfigurationFilePath, cancellationToken);
            var data = JsonSerializer.Deserialize<DevicesConfig>(json, JsonSerializerOptions)!;
            logger.LogInformation("device configuration file found. {PhysicalDeviceCount},{VirtualInfraredRemoteDevicesCount}", data.PhysicalDevices.Count, data.VirtualInfraredRemoteDevices.Count);
            return data;
        }
        logger.LogInformation("device configuration file missing.");
        return new DevicesConfig();
    }

    public async Task SaveFileAsync(DevicesConfig data, CancellationToken cancellationToken = default)
    {
        data.VirtualInfraredRemoteDevices.ForEach(d => d.ConfigureStatus = ConfigureStatus.NoChange);
        data.PhysicalDevices.ForEach(d => d.ConfigureStatus = ConfigureStatus.NoChange);
        Directory.CreateDirectory(Path.GetDirectoryName(DeviceConfigurationFilePath)!);
        if (File.Exists(DeviceConfigurationFilePath))
        {
            var dest = Path.Combine(Path.GetDirectoryName(DeviceConfigurationFilePath)!, $"switchbot_config_{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.json");
            File.Copy(DeviceConfigurationFilePath, dest);
            logger.LogInformation("old device configuration file renamed to {oldfilepath}", dest);
        }
        var json = JsonSerializer.Serialize(data, JsonSerializerOptions);
        await File.WriteAllTextAsync(DeviceConfigurationFilePath, json, Encoding.UTF8, cancellationToken);
        logger.LogInformation("device configuration file saved.");
    }
    #endregion

    #region SwitchBotApi
    public async Task LoadDevicesAsync(DevicesConfig currentData, CancellationToken cancellationToken = default)
    {
        try
        {
            var (response, responseRaw) = await switchBotApiClient.GetDevicesAsync(cancellationToken);

            foreach (var enforceDeviceType in enforceDeviceTypeOptions.Value)
            {
                foreach (var device in response.DeviceList.Where(s => s.DeviceId == enforceDeviceType.DeviceId))
                {
                    device.DeviceType = enforceDeviceType.DeviceType;
                }
                foreach (var device in response.InfraredRemoteList.Where(s => s.DeviceId == enforceDeviceType.DeviceId))
                {
                    device.RemoteType = enforceDeviceType.DeviceType;
                }
            }

            List<PhysicalDevice> physicalDevices = [];
            foreach (var d in response.DeviceList)
            {
                var deviceType = deviceDefinitionsManager.DeviceDefinitions.FirstOrDefault(dd => dd.ApiDeviceTypeString == d.DeviceType)?.DeviceType;
                if (deviceType == null)
                {
                    logger.LogWarning("{Method} unknown device type. {MacAddress},{DeviceType},{DeviceName}", nameof(LoadDevicesAsync), d.DeviceId, d.DeviceType, d.DeviceName);
                }
                else
                {
                    physicalDevices.Add(new PhysicalDevice()
                    {
                        DeviceId = d.DeviceId,
                        DeviceName = d.DeviceName,
                        DeviceType = deviceType!.Value,
                        Description = string.Empty,
                        RawValue = responseRaw.DeviceList.Where(rd => rd!.AsObject()["deviceId"]!.AsValue().GetValue<string>() == d.DeviceId).FirstOrDefault()?.AsObject()
                    });
                }
            }
            physicalDevices.ForEach(d =>
            {
                d.RawValue?.Remove("deviceId");
                d.RawValue?.Remove("deviceName");
            });
            Diff(physicalDevices, currentData.PhysicalDevices, device =>
            {
                var fields = deviceDefinitionsManager.FieldDefinitions.Where(m => m.DeviceType == device.DeviceType);
                device.Fields = fields.Select(f => new FieldConfig()
                {
                    Enable = true,
                    FieldName = f.FieldName,
                }).ToList();
                var commands = deviceDefinitionsManager.CommandDefinitions.Where(m => m.DeviceType == device.DeviceType);
                device.Commands = commands.Select(
                    c => new CommandConfig()
                    {
                        Enable = true,
                        CommandType = c.CommandType,
                        Command = c.Command,
                        DisplayName = c.DisplayName ?? $"{c.Command}",
                    }).ToList();
            });

            List<VirtualInfraredRemoteDevice> remoteDevices = response.InfraredRemoteList.Select(d => new VirtualInfraredRemoteDevice()
            {
                DeviceId = d.DeviceId,
                DeviceName = d.DeviceName,
                DeviceType = deviceDefinitionsManager.DeviceDefinitions.FirstOrDefault(dd => dd.ApiDeviceTypeString == d.RemoteType)?.DeviceType
                            ?? deviceDefinitionsManager.DeviceDefinitions.First(dd => dd.CustomizedDeviceTypeString == d.RemoteType).DeviceType,
                IsCustomized = !deviceDefinitionsManager.DeviceDefinitions.Any(dd => dd.ApiDeviceTypeString == d.RemoteType),
                RawValue = responseRaw.InfraredRemoteList.Where(rd => rd!.AsObject()["deviceId"]!.AsValue().GetValue<string>() == d.DeviceId).FirstOrDefault()?.AsObject()
            }).ToList();
            remoteDevices.ForEach(d =>
            {
                d.RawValue?.Remove("deviceId");
                d.RawValue?.Remove("deviceName");
            });
            Diff(remoteDevices, currentData.VirtualInfraredRemoteDevices, device =>
            {
                var master = deviceDefinitionsManager.DeviceDefinitions.First(m => m.DeviceType == device.DeviceType);
                var commands = deviceDefinitionsManager.CommandDefinitions.Where(m => m.DeviceType == device.DeviceType);
                device.Commands = commands.Select(
                    c => new CommandConfig()
                    {
                        Enable = true,
                        CommandType = c.CommandType,
                        Command = c.Command,
                        DisplayName = c.DisplayName ?? $"{c.Command}",
                    }).ToList();
            });

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Method} error.", nameof(LoadDevicesAsync));
        }
    }

    private static void Diff<T>(List<T> currentDevices, List<T> configuredDevices, Action<T> appendDefaults) where T : DeviceBase
    {
        foreach (var currentDevice in currentDevices)
        {
            var configuredDevice = configuredDevices.SingleOrDefault(d => d.DeviceId == currentDevice.DeviceId && d.ConfigureStatus != ConfigureStatus.Deleting);
            if (configuredDevice == null)
            {
                currentDevice.ConfigureStatus = ConfigureStatus.New;
                appendDefaults(currentDevice);
                configuredDevices.Add(currentDevice);
            }
            else if (currentDevice.DeviceName != configuredDevice.DeviceName)
            {
                configuredDevice.DeviceName = currentDevice.DeviceName;
                configuredDevice.ConfigureStatus = ConfigureStatus.Modified;
                configuredDevice.RawValue = currentDevice.RawValue;
            }
            else
            {
                configuredDevice.ConfigureStatus = ConfigureStatus.NoChange;
                configuredDevice.RawValue = currentDevice.RawValue;
            }
        }
        configuredDevices.Where(d =>
            !currentDevices.Any(md => d.DeviceId == md.DeviceId)
        ).ToList().ForEach(d => d.ConfigureStatus = ConfigureStatus.Missing);
    }
    #endregion
}
