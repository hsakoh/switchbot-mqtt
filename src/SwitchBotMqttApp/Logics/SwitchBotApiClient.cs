using Microsoft.Extensions.Options;
using SwitchBotMqttApp.Configurations;
using SwitchBotMqttApp.Models.DeviceConfiguration;
using SwitchBotMqttApp.Models.SwitchBotApi;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SwitchBotMqttApp.Logics;

/// <summary>
/// Client for communicating with SwitchBot Cloud API v1.1.
/// Handles authentication, device control, status retrieval, webhook management, and scene execution.
/// </summary>
public class SwitchBotApiClient
{
    private readonly ILogger<SwitchBotApiClient> _logger;
    private readonly HttpClient _switchBotHttpClient;
    private readonly IOptions<SwitchBotOptions> _switchBotOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchBotApiClient"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="httpClientFactory">HTTP client factory for creating SwitchBot API clients.</param>
    /// <param name="switchBotOptions">SwitchBot API configuration options.</param>
    public SwitchBotApiClient(
        ILogger<SwitchBotApiClient> logger
        , IHttpClientFactory httpClientFactory
        , IOptions<SwitchBotOptions> switchBotOptions)
    {
        _logger = logger;
        _switchBotOptions = switchBotOptions;
        _switchBotHttpClient = httpClientFactory.CreateClient(nameof(SwitchBotApiClient));
        _switchBotHttpClient.BaseAddress = new Uri(_switchBotOptions.Value.ApiBaseUrl);
        _switchBotHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Dictionary tracking API call counts per day for rate limit monitoring.
    /// SwitchBot API has a limit of 10,000 calls per day.
    /// </summary>
    private readonly ConcurrentDictionary<DateOnly, int> ApiCallCount = new();
    
    /// <summary>
    /// Gets the API call count for a specific date.
    /// </summary>
    /// <param name="dateOnly">Date to get call count for.</param>
    /// <returns>Number of API calls made on the specified date.</returns>
    public int GetApiCallCount(DateOnly dateOnly) => ApiCallCount.GetOrAdd(dateOnly, 0);

    /// <summary>
    /// Retrieves the list of all devices (physical and virtual IR remotes) from SwitchBot Cloud.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A tuple containing parsed device response and raw response data.</returns>
    public async Task<(GetDevicesResponseBody response, GetDevicesResponseBodyRaw responseRaw)> GetDevicesAsync(CancellationToken cancellationToken = default)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Get, "devices");
        var (body, raw) = await SendAndEnsureSuccessAsync<GetDevicesResponseBody>(requestMessage, cancellationToken);
        var responseRaw = JsonSerializer.Deserialize<SwitchBotResponse<GetDevicesResponseBodyRaw>>(raw)!;
        return (body, responseRaw.Body);

    }

    /// <summary>
    /// Gets the current status of a specific device.
    /// </summary>
    /// <param name="deviceId">Device MAC address or identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>JSON node containing device status information.</returns>
    public async Task<JsonNode> GetDeviceStatus(string deviceId, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Get, $"devices/{deviceId}/status");
        var (body, _) = await SendAndEnsureSuccessAsync<JsonNode>(requestMessage, cancellationToken);
        return body;
    }

    /// <summary>
    /// Queries all configured webhook URLs from SwitchBot Cloud.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Response containing list of webhook URLs.</returns>
    public async Task<QueryWebhooksResponse> GetWebhooksAsync(CancellationToken cancellationToken = default)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, "webhook/queryWebhook");

        var json = JsonSerializer.Serialize(
            new
            {
                action = "queryUrl"
            });
        requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
        var (body, _) = await SendAndEnsureSuccessAsync<QueryWebhooksResponse>(requestMessage, cancellationToken, [StatusCode.SystemError], bodyForLogging: json);
        body.Urls ??= [];
        return body;
    }

    /// <summary>
    /// Gets detailed information about specific webhooks.
    /// </summary>
    /// <param name="webhookUrls">Array of webhook URLs to query.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Array of webhook details including enabled status and device subscriptions.</returns>
    public async Task<QueryWebhookDetail[]> GetWebhookAsync(string[] webhookUrls, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, "webhook/queryWebhook");

        var json = JsonSerializer.Serialize(
            new
            {
                action = "queryDetails"
                ,
                urls = webhookUrls
            });
        requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
        var (body, _) = await SendAndEnsureSuccessAsync<QueryWebhookDetail[]>(requestMessage, cancellationToken, [StatusCode.SystemError], bodyForLogging: json);
        return body ?? [];
    }

    /// <summary>
    /// Updates the enabled status of a webhook.
    /// </summary>
    /// <param name="webhookUrl">Webhook URL to update.</param>
    /// <param name="enable">True to enable the webhook, false to disable.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateWebhookAsync(string webhookUrl, bool enable, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, "webhook/updateWebhook");

        var json = JsonSerializer.Serialize(
            new
            {
                action = "updateWebhook",
                config = new
                {
                    url = webhookUrl,
                    enable,
                }
            });
        requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
        _ = await SendAndEnsureSuccessAsync<object>(requestMessage, cancellationToken, bodyForLogging: json);
    }

    /// <summary>
    /// Configures a new webhook to receive device event notifications.
    /// Subscribes to all devices by default.
    /// </summary>
    /// <param name="webhookUrl">Webhook URL endpoint to receive notifications.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ConfigureWebhook(string webhookUrl, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, "webhook/setupWebhook");

        var json = JsonSerializer.Serialize(
            new
            {
                action = "setupWebhook",
                url = webhookUrl,
                deviceList = "ALL",
            });
        requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
        _ = await SendAndEnsureSuccessAsync<object>(requestMessage, cancellationToken, bodyForLogging: json);
    }

    /// <summary>
    /// Deletes a configured webhook from SwitchBot Cloud.
    /// </summary>
    /// <param name="webhookUrl">Webhook URL to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteWebhookAsync(string webhookUrl, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, "webhook/deleteWebhook");

        var json = JsonSerializer.Serialize(
            new
            {
                action = "deleteWebhook",
                url = webhookUrl,
            });
        requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
        _ = await SendAndEnsureSuccessAsync<object>(requestMessage, cancellationToken, bodyForLogging: json);
    }

    /// <summary>
    /// Sends an API request to SwitchBot and ensures successful response.
    /// Handles error codes and response validation.
    /// </summary>
    /// <typeparam name="TBody">Type of response body.</typeparam>
    /// <param name="requestMessage">HTTP request message.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <param name="additionalStatus">Additional status codes to accept as valid.</param>
    /// <param name="bodyForLogging">Request body for logging purposes.</param>
    /// <returns>Tuple containing parsed body and raw response string.</returns>
    private async Task<(TBody body, string raw)> SendAndEnsureSuccessAsync<TBody>(HttpRequestMessage requestMessage, CancellationToken cancellationToken, int[]? additionalStatus = null, string? bodyForLogging = null)
    {
        _logger.LogTrace("Send {Method},{Url},{Body}", requestMessage.Method, requestMessage.RequestUri, bodyForLogging);
        
        // Track API call count for rate limit monitoring
        ApiCallCount.AddOrUpdate(DateOnly.FromDateTime(DateTime.UtcNow), 1, (_, i) => { return i += 1; });
        
        using var responseMessage = await _switchBotHttpClient.SendAsync(requestMessage, cancellationToken);
        var responseString = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        var responseObject = JsonSerializer.Deserialize<SwitchBotResponse<TBody>>(responseString)!;
        _logger.LogTrace("Response {Method},{Url},{Response}", requestMessage.Method, requestMessage.RequestUri, responseString);

        // Check HTTP status code
        if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new ApplicationException(responseString);
        }
        
        // Check SwitchBot API status code
        if (responseObject.StatusCode != StatusCode.Success)
        {
            // Allow additional status codes if specified (e.g., SystemError for webhook queries)
            if (additionalStatus == null || !additionalStatus.Contains(responseObject.StatusCode))
            {
                throw new ApplicationException(responseString);
            }
        }
        return (responseObject.Body, responseString);
    }

    /// <summary>
    /// Creates an authenticated HTTP request for SwitchBot API with HMAC signature.
    /// Generates required authentication headers: Authorization, sign, nonce, and timestamp.
    /// </summary>
    /// <param name="method">HTTP method (GET, POST).</param>
    /// <param name="requestUri">API endpoint path.</param>
    /// <returns>Configured HTTP request message with authentication headers.</returns>
    private HttpRequestMessage CreateSwitchBotRequest(HttpMethod method, string requestUri)
    {
        try
        {
            HttpRequestMessage requestMessage = new(method, requestUri);
            
            // Generate HMAC-SHA256 signature for authentication
            var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var nonce = Guid.NewGuid().ToString();
            var data = $"{_switchBotOptions.Value.ApiKey}{time}{nonce}";
            using HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(_switchBotOptions.Value.ApiSecret));
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
            
            // Add required authentication headers
            requestMessage.Headers.TryAddWithoutValidation("Authorization", _switchBotOptions.Value.ApiKey);
            requestMessage.Headers.TryAddWithoutValidation("sign", signature);
            requestMessage.Headers.TryAddWithoutValidation("nonce", nonce);
            requestMessage.Headers.TryAddWithoutValidation("t", time.ToString());
            return requestMessage;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "signature generation error. ApiKey:{ApiKey},ApiSecret:{ApiSecret}", _switchBotOptions.Value.ApiSecret, _switchBotOptions.Value.ApiKey);
            throw;
        }
    }

    /// <summary>
    /// Sends a default device control command (parameter: "default").
    /// </summary>
    /// <param name="device">Target device.</param>
    /// <param name="command">Command configuration to execute.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Raw API response string.</returns>
    public async Task<string?> SendDefaultDeviceControlCommandAsync(DeviceBase device, CommandConfig command, CancellationToken cancellationToken)
    {
        return await SendDeviceControlCommandAsync(device, command, "default", cancellationToken);
    }

    /// <summary>
    /// Sends a device control command with custom parameter value.
    /// Supports various parameter types including primitives, JSON objects, and formatted strings.
    /// </summary>
    /// <param name="device">Target device.</param>
    /// <param name="command">Command configuration to execute.</param>
    /// <param name="value">Command parameter value (can be string, number, JSON object, etc.).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Raw API response string.</returns>
    public async Task<string?> SendDeviceControlCommandAsync(DeviceBase device, CommandConfig command, object value, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, $"devices/{device.DeviceId}/commands");

        var json = JsonSerializer.Serialize(
            new
            {
                commandType = command.CommandType.ToEnumMemberValue(),
                command = command.Command,
                parameter = value,
            });
        requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
        var (_, raw) = await SendAndEnsureSuccessAsync<object>(requestMessage, cancellationToken, bodyForLogging: json);
        return raw;
    }

    /// <summary>
    /// Retrieves the list of manual scenes from SwitchBot Cloud.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Array of scene definitions including scene IDs and names.</returns>
    public async Task<GetSceneListBody[]> GetSceneList(CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Get, "scenes");
        var (body, _) = await SendAndEnsureSuccessAsync<GetSceneListBody[]>(requestMessage, cancellationToken);
        return body;
    }

    /// <summary>
    /// Executes a manual scene by its ID.
    /// </summary>
    /// <param name="sceneId">Scene identifier to execute.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteManualSceneAsync(string sceneId, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, $"scenes/{sceneId}/execute");
        var (_, _) = await SendAndEnsureSuccessAsync<object>(requestMessage, cancellationToken);
    }
}
