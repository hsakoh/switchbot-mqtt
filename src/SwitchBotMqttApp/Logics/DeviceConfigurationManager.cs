using HomeAssistantAddOn.Core;
using Microsoft.Extensions.Options;
using SwitchBotMqttApp.Configurations;
using SwitchBotMqttApp.Models.DeviceConfiguration;
using System.Text;
using System.Text.Json;

namespace SwitchBotMqttApp.Logics;

/// <summary>
/// Manages device configuration including loading devices from SwitchBot API,
/// persisting configuration to file, and tracking configuration changes.
/// </summary>
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
    /// <summary>
    /// Reads device configuration from file.
    /// Creates a new empty configuration if file does not exist.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Device configuration containing physical and virtual IR remote devices.</returns>
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

    /// <summary>
    /// Saves device configuration to file with backup of previous configuration.
    /// Resets all device configuration status to NoChange after save.
    /// </summary>
    /// <param name="data">Device configuration to save.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
    /// <summary>
    /// Loads devices from SwitchBot Cloud API and merges with existing configuration.
    /// Tracks changes (new, modified, missing) and initializes default field/command configurations.
    /// </summary>
    /// <param name="currentData">Current device configuration to merge with.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task LoadDevicesAsync(DevicesConfig currentData, CancellationToken cancellationToken = default)
    {
        try
        {
            // Fetch devices from SwitchBot Cloud API
            var (response, responseRaw) = await switchBotApiClient.GetDevicesAsync(cancellationToken);

            // Apply device type overrides from configuration
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

            // Process physical devices
            List<PhysicalDevice> physicalDevices = [];
            foreach (var d in response.DeviceList)
            {
                // Map API device type string to enum
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
            // Remove redundant properties from raw value
            physicalDevices.ForEach(d =>
            {
                d.RawValue?.Remove("deviceId");
                d.RawValue?.Remove("deviceName");
            });
            
            // Compare with existing configuration and initialize defaults for new devices
            Diff(physicalDevices, currentData.PhysicalDevices, device =>
            {
                // Initialize field configuration from device definition
                var fields = deviceDefinitionsManager.DeviceDefinitions.First(m => m.DeviceType == device.DeviceType).Fields;
                device.Fields = [.. fields.Select(f => new FieldConfig()
                {
                    Enable = true,
                    FieldName = f.FieldName,
                })];
                
                // Initialize command configuration from device definition
                var commands = deviceDefinitionsManager.DeviceDefinitions.First(m => m.DeviceType == device.DeviceType).Commands;
                device.Commands = [.. commands.Select(
                    c => new CommandConfig()
                    {
                        Enable = true,
                        CommandType = c.CommandType,
                        Command = c.Command,
                        DisplayName = c.DisplayName ?? $"{c.Command}",
                    })];
            });

            // Process virtual infrared remote devices
            List<VirtualInfraredRemoteDevice> remoteDevices = [.. response.InfraredRemoteList.Select(d => new VirtualInfraredRemoteDevice()
            {
                DeviceId = d.DeviceId,
                DeviceName = d.DeviceName,
                // Map to device type, check both API type and customized type
                DeviceType = deviceDefinitionsManager.DeviceDefinitions.FirstOrDefault(dd => dd.ApiDeviceTypeString == d.RemoteType)?.DeviceType
                            ?? deviceDefinitionsManager.DeviceDefinitions.First(dd => dd.CustomizedDeviceTypeString == d.RemoteType).DeviceType,
                IsCustomized = !deviceDefinitionsManager.DeviceDefinitions.Any(dd => dd.ApiDeviceTypeString == d.RemoteType),
                RawValue = responseRaw.InfraredRemoteList.Where(rd => rd!.AsObject()["deviceId"]!.AsValue().GetValue<string>() == d.DeviceId).FirstOrDefault()?.AsObject()
            })];
            // Remove redundant properties from raw value
            remoteDevices.ForEach(d =>
            {
                d.RawValue?.Remove("deviceId");
                d.RawValue?.Remove("deviceName");
            });
            
            // Compare with existing configuration and initialize defaults for new devices
            Diff(remoteDevices, currentData.VirtualInfraredRemoteDevices, device =>
            {
                // Initialize command configuration from device definition
                var master = deviceDefinitionsManager.DeviceDefinitions.First(m => m.DeviceType == device.DeviceType);
                var commands = master.Commands;
                device.Commands = [.. commands.Select(
                    c => new CommandConfig()
                    {
                        Enable = true,
                        CommandType = c.CommandType,
                        Command = c.Command,
                        DisplayName = c.DisplayName ?? $"{c.Command}",
                    })];
            });

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Method} error.", nameof(LoadDevicesAsync));
        }
    }

    /// <summary>
    /// Compares current devices from API with configured devices and updates configuration status.
    /// Marks devices as New, Modified, NoChange, or Missing based on comparison.
    /// </summary>
    /// <typeparam name="T">Type of device (PhysicalDevice or VirtualInfraredRemoteDevice).</typeparam>
    /// <param name="currentDevices">Current devices retrieved from API.</param>
    /// <param name="configuredDevices">Existing configured devices.</param>
    /// <param name="appendDefaults">Action to append default field and command configurations for new devices.</param>
    private static void Diff<T>(List<T> currentDevices, List<T> configuredDevices, Action<T> appendDefaults) where T : DeviceBase
    {
        foreach (var currentDevice in currentDevices)
        {
            // Find existing configuration for this device
            var configuredDevice = configuredDevices.SingleOrDefault(d => d.DeviceId == currentDevice.DeviceId && d.ConfigureStatus != ConfigureStatus.Deleting);
            if (configuredDevice == null)
            {
                // New device found in API
                currentDevice.ConfigureStatus = ConfigureStatus.New;
                appendDefaults(currentDevice);
                configuredDevices.Add(currentDevice);
            }
            else if (currentDevice.DeviceName != configuredDevice.DeviceName)
            {
                // Device name changed
                configuredDevice.DeviceName = currentDevice.DeviceName;
                configuredDevice.ConfigureStatus = ConfigureStatus.Modified;
                configuredDevice.RawValue = currentDevice.RawValue;
            }
            else
            {
                // No changes
                configuredDevice.ConfigureStatus = ConfigureStatus.NoChange;
                configuredDevice.RawValue = currentDevice.RawValue;
            }
        }
        
        // Mark devices not found in API as missing
        configuredDevices.Where(d =>
            !currentDevices.Any(md => d.DeviceId == md.DeviceId)
        ).ToList().ForEach(d => d.ConfigureStatus = ConfigureStatus.Missing);
    }
    #endregion
}
