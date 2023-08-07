using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.SwitchBotApi;

public class GetDevicesResponseBodyRaw
{
    [JsonPropertyName("deviceList")]
    public JsonArray DeviceList { get; set; } = default!;
    [JsonPropertyName("infraredRemoteList")]
    public JsonArray InfraredRemoteList { get; set; } = default!;
}
