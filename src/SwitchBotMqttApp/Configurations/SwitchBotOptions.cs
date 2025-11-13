namespace SwitchBotMqttApp.Configurations;

/// <summary>
/// Configuration options for SwitchBot Cloud API authentication and connection.
/// </summary>
public class SwitchBotOptions
{
    /// <summary>
    /// Gets or sets the SwitchBot API key (Open Token) from SwitchBot app.
    /// Obtain from: SwitchBot App -> Profile -> Preferences -> App Version (tap 10 times) -> Developer Options.
    /// </summary>
    public string ApiKey { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the SwitchBot API secret key for HMAC authentication.
    /// Obtain from: SwitchBot App -> Profile -> Preferences -> App Version (tap 10 times) -> Developer Options.
    /// </summary>
    public string ApiSecret { get; set; } = default!;
    
    /// <summary>
    /// Gets or sets the base URL for SwitchBot Cloud API (default: https://api.switch-bot.com/v1.1/).
    /// </summary>
    public string ApiBaseUrl { get; set; } = default!;
}