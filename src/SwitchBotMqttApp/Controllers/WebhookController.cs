using Microsoft.AspNetCore.Mvc;
using SwitchBotMqttApp.Services;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Controllers;

public class WebhookController : Controller
{
    private readonly ILogger<WebhookController> _logger;
    private readonly MqttCoreService _mqttCoreService;

    public WebhookController(ILogger<WebhookController> logger
        , MqttCoreService mqttCoreService)
    {
        _logger = logger;
        _mqttCoreService = mqttCoreService;
    }

    [Route("/webhook")]
    [HttpPost]
    public async Task<IActionResult> WebhookAsync()
    {
        using var sr = new StreamReader(Request.Body);
        var json = await sr.ReadToEndAsync();
        try
        {
            _logger.LogInformation("{json}", json);
            var inputRaw = JsonSerializer.Deserialize<WebhookRaw>(json);
            await _mqttCoreService.PublishWebhookAsync(inputRaw!.Context);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "webhook action {Payload}", json);
            return StatusCode(500);
        }
    }

    public class WebhookRaw
    {
        [JsonPropertyName("eventType")]
        public string EventType { get; set; } = default!;
        [JsonPropertyName("eventVersion")]
        public string EventVersion { get; set; } = default!;
        [JsonPropertyName("context")]
        public JsonNode Context { get; set; } = default!;
    }
}
