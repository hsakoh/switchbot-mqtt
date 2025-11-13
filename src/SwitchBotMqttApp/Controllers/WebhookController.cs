using Microsoft.AspNetCore.Mvc;
using SwitchBotMqttApp.Services;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Controllers;

/// <summary>
/// API controller for receiving webhook notifications from SwitchBot Cloud.
/// Processes real-time device event notifications and publishes to MQTT.
/// </summary>
public class WebhookController(ILogger<WebhookController> logger
        , MqttCoreService mqttCoreService) : Controller
{
    /// <summary>
    /// Webhook endpoint that receives device event notifications from SwitchBot Cloud.
    /// </summary>
    /// <returns>OK response on success, 500 on error.</returns>
    [Route("/webhook")]
    [HttpPost]
    public async Task<IActionResult> WebhookAsync()
    {
        using var sr = new StreamReader(Request.Body);
        var json = await sr.ReadToEndAsync();
        try
        {
            logger.LogInformation("{json}", json);
            var inputRaw = JsonSerializer.Deserialize<WebhookRaw>(json);
            var inputRawRoot = JsonSerializer.Deserialize<JsonNode>(json);
            await mqttCoreService.PublishWebhookAsync(inputRaw!.Context, inputRawRoot!);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "webhook action {Payload}", json);
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Raw webhook payload structure from SwitchBot Cloud.
    /// </summary>
    public class WebhookRaw
    {
        /// <summary>
        /// Gets or sets the event type (e.g., "changeReport").
        /// </summary>
        [JsonPropertyName("eventType")]
        public string EventType { get; set; } = default!;
        
        /// <summary>
        /// Gets or sets the webhook event version.
        /// </summary>
        [JsonPropertyName("eventVersion")]
        public string EventVersion { get; set; } = default!;
        
        /// <summary>
        /// Gets or sets the context containing device event data.
        /// </summary>
        [JsonPropertyName("context")]
        public JsonNode Context { get; set; } = default!;
    }
}
