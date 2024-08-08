using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace HomeAssistantAddOn.Mqtt;

public class SupervisorApi(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(SupervisorApi));

    public async Task<SupoervisorResponse<ServiceMqtt>?> GetServicesMqtt()
    {
        return await _httpClient.GetFromJsonAsync<SupoervisorResponse<ServiceMqtt>>("/services/mqtt");
    }


    public class SupoervisorResponse<T> where T : class
    {
        [JsonPropertyName("result")]
        public string Result { get; set; } = default!;
        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;

    }

    public class ServiceMqtt
    {
        [JsonPropertyName("host")]
        public string Host { get; set; } = default!;
        [JsonPropertyName("port")]
        public int Port { get; set; } = default!;
        [JsonPropertyName("ssl")]
        public bool Ssl { get; set; } = default!;
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; } = default!;
        [JsonPropertyName("username")]
        public string Username { get; set; } = default!;
        [JsonPropertyName("password")]
        public string Password { get; set; } = default!;
        [JsonPropertyName("addon")]
        public string Addon { get; set; } = default!;
    }

    public class AddonInfo
    {
        [JsonPropertyName("ingress_entry")]
        public string IngressEntry { get; set; } = default!;
    }
}