using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.SwitchBotApi;

public class SwitchBotResponse<TBody>
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; } = default!;
    [JsonPropertyName("body")]
    public TBody Body { get; set; } = default!;
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;
}