namespace SwitchBotMqttApp.Configurations;

/// <summary>
/// Specifies the tunnel/connectivity mode used for receiving SwitchBot webhooks.
/// </summary>
public enum WebhookTunnelMode
{
    /// <summary>Webhook reception is disabled. Only polling will be used.</summary>
    Disabled,

    /// <summary>Use a manually configured public URL. Requires port-forwarding or equivalent setup.</summary>
    HostUrl,

    /// <summary>Use Ngrok tunnel. Requires an Ngrok auth token.</summary>
    Ngrok,

    /// <summary>Use Cloudflare TryCloudflare (no account needed, random URL issued).</summary>
    TryCloudflare,

    /// <summary>Use Cloudflare Zero Trust tunnel with a fixed URL. Requires a tunnel token.</summary>
    CloudflareZeroTrust,
}

/// <summary>
/// Configuration options for webhook service that receives real-time device event notifications from SwitchBot Cloud.
/// </summary>
public class WebhookServiceOptions
{
    /// <summary>
    /// Gets or sets the tunnel mode used for webhook reception.
    /// </summary>
    public WebhookTunnelMode TunnelMode { get; set; } = WebhookTunnelMode.Ngrok;

    /// <summary>
    /// Gets or sets the Ngrok authentication token for creating tunnels.
    /// Required when TunnelMode is <see cref="WebhookTunnelMode.Ngrok"/>.
    /// Get from https://dashboard.ngrok.com/get-started/your-authtoken.
    /// </summary>
    public string NgrokAuthToken { get; set; } = default!;

    /// <summary>
    /// Gets or sets the public webhook host URL (excluding /webhook path).
    /// Required when TunnelMode is <see cref="WebhookTunnelMode.HostUrl"/>.
    /// Also used as the public URL in <see cref="WebhookTunnelMode.CloudflareZeroTrust"/> mode.
    /// Must be publicly accessible by SwitchBot Cloud.
    /// </summary>
    public string HostUrl { get; set; } = default!;

    /// <summary>
    /// Gets or sets the Cloudflare Zero Trust tunnel token.
    /// Required when TunnelMode is <see cref="WebhookTunnelMode.CloudflareZeroTrust"/>.
    /// Obtain from the Cloudflare Zero Trust dashboard.
    /// </summary>
    public string CloudflareTunnelToken { get; set; } = default!;
}
