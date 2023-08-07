namespace HomeAssistantAddOn.Mqtt;
public class MqttOptions
{
    public bool UseAutoConfig { get; set; } = true;

    public string? Host { get; set; } = null;

    public int Port { get; set; } = 1883;

    public string? Id { get; set; } = null;

    public string? Pw { get; set; } = null;

    public bool Tls { get; set; } = false;
}