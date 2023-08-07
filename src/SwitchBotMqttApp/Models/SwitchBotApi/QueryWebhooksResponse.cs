using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.SwitchBotApi;

public class QueryWebhooksResponse
{
    [JsonPropertyName("urls")]
    public string[] Urls { get; set; } = default!;
}
