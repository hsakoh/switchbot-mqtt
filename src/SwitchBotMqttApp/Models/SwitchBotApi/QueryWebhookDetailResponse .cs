using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.SwitchBotApi;

public class QueryWebhookDetail
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;
    [JsonPropertyName("createTime")]
    public int CreateTime { get; set; }
    [JsonPropertyName("lastUpdateTime")]
    public int LastUpdateTime { get; set; }
    [JsonPropertyName("deviceList")]
    public string DeviceList { get; set; } = default!;
    [JsonPropertyName("enable")]
    public bool Enable { get; set; }
}
