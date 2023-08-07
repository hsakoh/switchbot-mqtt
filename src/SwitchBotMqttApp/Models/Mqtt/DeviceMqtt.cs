using Newtonsoft.Json;

namespace SwitchBotMqttApp.Models.Mqtt;


public class DeviceMqtt
{
    public DeviceMqtt(string[] identifiers, string name,
        string? manufacturer = null, string? model = null, string? hwVersion = null, string? swVersion = null, string? viaDevice = null)
    {
        Identifiers = identifiers;
        Name = name;
        Manufacturer = manufacturer;
        Model = model;
        HwVersion = hwVersion;
        SwVersion = swVersion;
        ViaDevice = viaDevice;
    }

    [JsonProperty("identifiers")]
    public string[] Identifiers { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("manufacturer")]
    public string? Manufacturer { get; set; }
    [JsonProperty("model")]
    public string? Model { get; set; }
    [JsonProperty("hw_version")]
    public string? HwVersion { get; set; }
    [JsonProperty("sw_version")]
    public string? SwVersion { get; set; }
    [JsonProperty("via_device")]
    public string? ViaDevice { get; set; }
}
