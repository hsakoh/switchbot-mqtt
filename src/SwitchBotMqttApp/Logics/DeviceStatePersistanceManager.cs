using HomeAssistantAddOn.Core;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;
using CommonOptions = SwitchBotMqttApp.Configurations.CommonOptions;

namespace SwitchBotMqttApp.Logics;

/// <summary>
/// Manages persistence of device state to file system for state retention across application restarts.
/// Stores device state as JSON files indexed by device ID.
/// </summary>
public class DeviceStatePersistanceManager(IOptions<CommonOptions> commonOption)
{
    /// <summary>
    /// Saves or updates device state to persistent storage.
    /// Merges new state properties with existing state.
    /// </summary>
    /// <param name="deviceId">Device identifier (MAC address).</param>
    /// <param name="newState">New state data to save or merge.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SaveAsync(string deviceId, JsonNode newState)
    {
        if (!commonOption.Value.DeviceStatePersistence)
        {
            return;
        }
        var filePath = GetStateFilePath(deviceId);
        var state = await LoadAsync(deviceId);
        foreach (var item in newState.AsObject())
        {
            if (item.Value != null)
            {
                state[item.Key] = item.Value.DeepClone();
            }
        }
        File.WriteAllText(filePath, JsonSerializer.Serialize(state));
    }

    /// <summary>
    /// Loads device state from persistent storage.
    /// Returns empty JSON object if state file doesn't exist or persistence is disabled.
    /// </summary>
    /// <param name="deviceId">Device identifier (MAC address).</param>
    /// <returns>JSON node containing device state data, or empty object if not found.</returns>
    public async Task<JsonNode> LoadAsync(string deviceId)
    {
        if (!commonOption.Value.DeviceStatePersistence)
        {
            return new JsonObject();
        }
        var filePath = GetStateFilePath(deviceId);
        if (File.Exists(filePath))
        {
            var json = await File.ReadAllTextAsync(GetStateFilePath(deviceId));
            try
            {
                return JsonNode.Parse(json)!;
            }
            catch (Exception)
            {

            }
        }
        return new JsonObject();
    }

    /// <summary>
    /// Gets the file path for storing device state based on device ID.
    /// </summary>
    /// <param name="deviceId">Device identifier (MAC address).</param>
    /// <returns>Full file path for device state JSON file.</returns>
    private string GetStateFilePath(string deviceId)
    {
        return Path.Combine(Utility.GetBaseDataDirectory(), $"{deviceId}.json");
    }
}
