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

public class SwitchBotApiClient
{
    private readonly ILogger<SwitchBotApiClient> _logger;
    private readonly HttpClient _switchBotHttpClient;
    private readonly IOptions<SwitchBotOptions> _switchBotOptions;

    public SwitchBotApiClient(
        ILogger<SwitchBotApiClient> logger
        , IHttpClientFactory httpClientFactory
        , IOptions<SwitchBotOptions> switchBotOptions)
    {
        _logger = logger;
        _switchBotOptions = switchBotOptions;
        _switchBotHttpClient = httpClientFactory.CreateClient(nameof(SwitchBotApiClient));
        _switchBotHttpClient.BaseAddress = new Uri($"https://api.switch-bot.com/v1.1/");
        _switchBotHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private readonly ConcurrentDictionary<DateOnly, int> ApiCallCount = new();
    public int GetApiCallCount(DateOnly dateOnly) => ApiCallCount.GetOrAdd(dateOnly, 0);

    public async Task<(GetDevicesResponseBody response, GetDevicesResponseBodyRaw responseRaw)> GetDevicesAsync(CancellationToken cancellationToken = default)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Get, "devices");
        var (body, raw) = await SendAndEnsureSuccessAsync<GetDevicesResponseBody>(requestMessage, cancellationToken);
        var responseRaw = JsonSerializer.Deserialize<SwitchBotResponse<GetDevicesResponseBodyRaw>>(raw)!;
        return (body, responseRaw.Body);

    }

    public async Task<JsonNode> GetDeviceStatus(string deviceId, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Get, $"devices/{deviceId}/status");
        var (body, _) = await SendAndEnsureSuccessAsync<JsonNode>(requestMessage, cancellationToken);
        return body;
    }

    public async Task<QueryWebhooksResponse> GetWebhooksAsync(CancellationToken cancellationToken = default)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, "webhook/queryWebhook");

        requestMessage.Content = new StringContent(JsonSerializer.Serialize(
            new
            {
                action = "queryUrl"
            }), Encoding.UTF8, "application/json");

        var (body, _) = await SendAndEnsureSuccessAsync<QueryWebhooksResponse>(requestMessage, cancellationToken, new int[] { StatusCode.SystemError });
        body.Urls ??= Array.Empty<string>();
        return body;
    }

    public async Task<QueryWebhookDetail[]> GetWebhookAsync(string[] webhookUrls, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, "webhook/queryWebhook");

        requestMessage.Content = new StringContent(JsonSerializer.Serialize(
            new
            {
                action = "queryDetails"
                ,
                urls = webhookUrls
            }), Encoding.UTF8, "application/json");

        var (body, _) = await SendAndEnsureSuccessAsync<QueryWebhookDetail[]>(requestMessage, cancellationToken, new int[] { StatusCode.SystemError });
        return body ?? Array.Empty<QueryWebhookDetail>();
    }

    public async Task UpdateWebhookAsync(string webhookUrl, bool enable, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, "webhook/updateWebhook");

        requestMessage.Content = new StringContent(JsonSerializer.Serialize(
            new
            {
                action = "updateWebhook",
                config = new
                {
                    url = webhookUrl,
                    enable,
                }
            }), Encoding.UTF8, "application/json");

        _ = await SendAndEnsureSuccessAsync<object>(requestMessage, cancellationToken);
    }

    public async Task ConfigureWebhook(string webhookUrl, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, "webhook/setupWebhook");

        requestMessage.Content = new StringContent(JsonSerializer.Serialize(
            new
            {
                action = "setupWebhook",
                url = webhookUrl,
                deviceList = "ALL",
            }), Encoding.UTF8, "application/json");
        _ = await SendAndEnsureSuccessAsync<object>(requestMessage, cancellationToken);
    }

    public async Task DeleteWebhookAsync(string webhookUrl, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, "webhook/deleteWebhook");

        requestMessage.Content = new StringContent(JsonSerializer.Serialize(
            new
            {
                action = "deleteWebhook",
                url = webhookUrl,
            }), Encoding.UTF8, "application/json");
        _ = await SendAndEnsureSuccessAsync<object>(requestMessage, cancellationToken);
    }

    private async Task<(TBody body, string raw)> SendAndEnsureSuccessAsync<TBody>(HttpRequestMessage requestMessage, CancellationToken cancellationToken, int[]? additionalStatus = null)
    {
        _logger.LogTrace("Send {Method},{Url}", requestMessage.Method, requestMessage.RequestUri);
        ApiCallCount.AddOrUpdate(DateOnly.FromDateTime(DateTime.UtcNow), 1, (_, i) => { return i += 1; });
        using var responseMessage = await _switchBotHttpClient.SendAsync(requestMessage, cancellationToken);
        var responseString = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        var responseObject = JsonSerializer.Deserialize<SwitchBotResponse<TBody>>(responseString)!;
        _logger.LogTrace("Response {Method},{Url},{Response}", requestMessage.Method, requestMessage.RequestUri, responseString);

        if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new ApplicationException(responseString);
        }
        if (responseObject.StatusCode != StatusCode.Success)
        {
            if (additionalStatus == null || !additionalStatus.Contains(responseObject.StatusCode))
            {
                throw new ApplicationException(responseString);
            }
        }
        return (responseObject.Body, responseString);
    }

    private HttpRequestMessage CreateSwitchBotRequest(HttpMethod method, string requestUri)
    {
        try
        {
            HttpRequestMessage requestMessage = new(method, requestUri);
            var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var nonce = Guid.NewGuid().ToString();
            var data = $"{_switchBotOptions.Value.ApiKey}{time}{nonce}";
            using HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(_switchBotOptions.Value.ApiSecret));
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
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

    public async Task<string?> SendDefaultDeviceControlCommandAsync(DeviceBase device, CommandConfig command, CancellationToken cancellationToken)
    {
        return await SendDeviceControlCommandAsync(device, command, "default", cancellationToken);
    }

    public async Task<string?> SendDeviceControlCommandAsync(DeviceBase device, CommandConfig command, object value, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, $"devices/{device.DeviceId}/commands");

        requestMessage.Content = new StringContent(JsonSerializer.Serialize(
            new
            {
                commandType = command.CommandType.ToEnumMemberValue(),
                command = command.Command,
                parameter = value,
            }), Encoding.UTF8, "application/json");
        var (_, raw) = await SendAndEnsureSuccessAsync<object>(requestMessage, cancellationToken);
        return raw;
    }

    public async Task<GetSceneListBody[]> GetSceneList(CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Get, "scenes");
        var (body, _) = await SendAndEnsureSuccessAsync<GetSceneListBody[]>(requestMessage, cancellationToken);
        return body;
    }

    public async Task ExecuteManualSceneAsync(string sceneId, CancellationToken cancellationToken)
    {
        using var requestMessage = CreateSwitchBotRequest(HttpMethod.Post, $"scenes/{sceneId}/execute");
        var (_, _) = await SendAndEnsureSuccessAsync<object>(requestMessage, cancellationToken);
    }
}
