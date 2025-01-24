using HomeAssistantAddOn.Core;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;
using CommonOptions = SwitchBotMqttApp.Configurations.CommonOptions;

namespace SwitchBotMqttApp.Logics;

public class DeviceStatePersistanceManager(IOptions<CommonOptions> commonOption)
{
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

    private string GetStateFilePath(string deviceId)
    {
        return Path.Combine(Utility.GetBaseDataDirectory(), $"{deviceId}.json");
    }
}
