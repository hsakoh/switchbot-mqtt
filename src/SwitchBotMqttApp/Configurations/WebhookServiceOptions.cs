namespace SwitchBotMqttApp.Configurations;

public class WebhookServiceOptions
{
    public bool UseWebhook { get; set; } = true;
    public bool UseNgrok { get; set; } = true;
    public string NgrokAuthToken { get; set; } = default!;
    public string HostUrl { get; set; } = default!;

}
