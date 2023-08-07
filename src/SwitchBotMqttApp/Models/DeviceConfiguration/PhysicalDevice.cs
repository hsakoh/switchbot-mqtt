using Microsoft.AspNetCore.Components;
using System.Text.Json.Serialization;
using System.Xml;

namespace SwitchBotMqttApp.Models.DeviceConfiguration;

public class PhysicalDevice : DeviceBase
{

    public bool UsePolling
    {
        get; set;
    }

    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromHours(1);

    [JsonIgnore]
    public string PollingIntervalProxy
    {
        get => XmlConvert.ToString(PollingInterval);
        set
        {
            try
            {
                var temp = XmlConvert.ToTimeSpan(value);
                if (PollingInterval != temp)
                {
                    PollingInterval = temp;
                    PollingIntervalProxyChanged.InvokeAsync(PollingIntervalProxy);
                }
            }
            catch (Exception)
            {
            }
        }
    }

    public bool UseWebhook { get; set; }

    public EventCallback<string> PollingIntervalProxyChanged;
    public async Task ChangePollingIntervalProxyAsync(ChangeEventArgs changeEventArgs)
    {
        PollingIntervalProxy = (string)changeEventArgs.Value!;
        await PollingIntervalProxyChanged.InvokeAsync(PollingIntervalProxy);
    }

    public List<FieldConfig> Fields { get; set; } = default!;
}