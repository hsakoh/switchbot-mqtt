namespace SwitchBotMqttApp.Configurations;

/// <summary>
/// Configuration options for webhook service that receives real-time device event notifications from SwitchBot Cloud.
/// </summary>
public class WebhookServiceOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether webhook functionality is enabled.
    /// When disabled, only polling will be used for device status updates.
    /// </summary>
    public bool UseWebhook { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to use Ngrok for webhook tunneling.
    /// Useful for development/testing when public URL is not available.
    /// </summary>
    public bool UseNgrok { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the Ngrok authentication token for creating tunnels.
    /// Required when UseNgrok is true. Get from https://dashboard.ngrok.com/get-started/your-authtoken.
    /// </summary>
    public string NgrokAuthToken { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the public webhook URL endpoint.
    /// Used when UseNgrok is false. Must be publicly accessible by SwitchBot Cloud.
    /// </summary>
    public string HostUrl { get; set; } = default!;
}
