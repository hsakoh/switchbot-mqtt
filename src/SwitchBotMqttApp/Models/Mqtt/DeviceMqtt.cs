using Newtonsoft.Json;

namespace SwitchBotMqttApp.Models.Mqtt;


public class DeviceMqtt(string[] identifiers, string name,
    string? manufacturer = null, string? model = null, string? hwVersion = null, string? swVersion = null, string? viaDevice = null)
{
    [JsonProperty("identifiers")]
    public string[] Identifiers { get; set; } = identifiers;
    [JsonProperty("name")]
    public string Name { get; set; } = name;

    [JsonProperty("manufacturer")]
    public string? Manufacturer { get; set; } = manufacturer;
    [JsonProperty("model")]
    public string? Model { get; set; } = model;
    [JsonProperty("hw_version")]
    public string? HwVersion { get; set; } = hwVersion;
    [JsonProperty("sw_version")]
    public string? SwVersion { get; set; } = swVersion;
    [JsonProperty("via_device")]
    public string? ViaDevice { get; set; } = viaDevice;
}
