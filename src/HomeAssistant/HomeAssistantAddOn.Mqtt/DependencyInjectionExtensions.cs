using HomeAssistantAddOn.Mqtt;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddHomeAssistantMqtt(
        this IServiceCollection services)
    {
        services.AddOptions<MqttOptions>().
            Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("Mqtt").Bind(settings);
            });
        services.AddSingleton<MqttService>();
        services.AddSingleton<SupervisorApi>();
        services.AddHttpClient(nameof(SupervisorApi), httpClient =>
        {
            httpClient.BaseAddress = new Uri("http://supervisor/");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Environment.GetEnvironmentVariable("SUPERVISOR_TOKEN")! ?? "invalid");
        });
        return services;
    }
}
