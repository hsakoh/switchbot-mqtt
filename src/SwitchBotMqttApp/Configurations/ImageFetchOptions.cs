namespace SwitchBotMqttApp.Configurations;

public class ImageFetchOptions
{
    public int MaxRetries { get; set; } = 25;
    public int RetryIntervalMs { get; set; } = 1000;
}
