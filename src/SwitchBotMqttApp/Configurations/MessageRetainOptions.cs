namespace SwitchBotMqttApp.Configurations;

public class MessageRetainOptions
{
    public bool Entity { get; set; } = true;
    public bool State { get; set; } = false;
}
