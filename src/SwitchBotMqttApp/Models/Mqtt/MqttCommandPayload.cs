namespace SwitchBotMqttApp.Models.Mqtt;

public class MqttCommandPayload
{
    public string CommandType { get; set; } = default!;
    public string Command { get; set; } = default!;
    public string ParamName { get; set; } = default!;
    public string ParamValue { get; set; } = default!;
}