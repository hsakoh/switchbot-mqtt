using Microsoft.AspNetCore.Components;
using System.Text.Json.Serialization;
using System.Xml;

namespace SwitchBotMqttApp.Models.DeviceConfiguration;

/// <summary>
/// Configuration for physical SwitchBot devices that support status polling and webhook notifications.
/// </summary>
public class PhysicalDevice : DeviceBase
{
    /// <summary>
    /// Gets or sets a value indicating whether periodic polling is enabled for this device.
    /// </summary>
    public bool UsePolling
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the interval between polling requests for device status updates.
    /// </summary>
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets the polling interval as an ISO 8601 duration string for UI binding.
    /// Not persisted to JSON.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether webhook notifications are enabled for this device.
    /// </summary>
    public bool UseWebhook { get; set; }

    /// <summary>
    /// Event callback for polling interval changes in Blazor UI.
    /// </summary>
    public EventCallback<string> PollingIntervalProxyChanged;
    
    /// <summary>
    /// Handles polling interval change events from UI components.
    /// </summary>
    /// <param name="changeEventArgs">Change event arguments containing new value.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ChangePollingIntervalProxyAsync(ChangeEventArgs changeEventArgs)
    {
        PollingIntervalProxy = (string)changeEventArgs.Value!;
        await PollingIntervalProxyChanged.InvokeAsync(PollingIntervalProxy);
    }

    /// <summary>
    /// Gets or sets the list of status fields (sensors) available for this physical device.
    /// </summary>
    public List<FieldConfig> Fields { get; set; } = default!;
}