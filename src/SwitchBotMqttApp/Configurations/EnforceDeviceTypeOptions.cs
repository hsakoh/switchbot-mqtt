namespace SwitchBotMqttApp.Configurations;

public class EnforceDeviceTypeOptions : List<EnforceDeviceType>
{

}
public class EnforceDeviceType
{
    public string DeviceId { get; set; } = default!;
    public string DeviceType { get; set; } = default!;
}