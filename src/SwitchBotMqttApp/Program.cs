using Blazored.Modal;
using FluffySpoon.Ngrok;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using SwitchBotMqttApp.Configurations;
using SwitchBotMqttApp.Logics;
using SwitchBotMqttApp.Services;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using static HomeAssistantAddOn.Mqtt.SupervisorApi;

namespace SwitchBotMqttApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(8098);//ForWebhook
            serverOptions.ListenAnyIP(8099);//ForWebUI
        });
        builder.Configuration.AddHomeAssistantAddOnConfig();

        var config = builder.Configuration.Get<HomeAssistantAddOn.Core.CommonOptions>();
        builder.Logging
            .Configure(options =>
            {
                options.ActivityTrackingOptions = ActivityTrackingOptions.None;
            })
            .AddFilter("SwitchBotMqttApp", config!.LogLevel)
            .AddSimpleConsole(options =>
            {
                options.IncludeScopes = false;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddBlazoredModal();
        builder.Services.AddOptions<CommonOptions>().
            Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.Bind(settings);
            });
        builder.Services.AddOptions<SwitchBotOptions>().
            Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("SwitchBot").Bind(settings);
            });
        builder.Services.AddOptions<EnforceDeviceTypeOptions>().
            Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("EnforceDeviceTypes").Bind(settings);
            });
        builder.Services.AddOptions<WebhookServiceOptions>().
            Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("WebhookService").Bind(settings);
            });

        builder.Services.AddSingleton<DeviceConfigurationManager>();
        builder.Services.AddSingleton<DeviceDefinitionsManager>();
        builder.Services.AddSingleton<SwitchBotApiClient>();
        builder.Services.AddHttpContextAccessor();

#if DEBUG
        builder.Services.AddHttpClient(nameof(SwitchBotApiClient))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                Proxy = new WebProxy("localhost", 8888)
            });
        //builder.Services.AddHttpClient(nameof(SwitchBotApiClient));
#else
        builder.Services.AddHttpClient(nameof(SwitchBotApiClient));
#endif

        builder.Services.AddNgrok(options =>
        {
            var config = builder.Configuration.GetSection("WebhookService").Get<WebhookServiceOptions>()!;
            if (config.UseNgrok)
            {
                options.AuthToken = config.NgrokAuthToken;
#if DEBUG
                options.ShowNgrokWindow = true;
#else
                options.ShowNgrokWindow = false;
#endif
            }
        });
        builder.Services.AddSingleton<MqttCoreService>();
        builder.Services.AddSingleton<PollingService>();
        builder.Services.AddSingleton<WebhookService>();
        builder.Services.AddHostedService<AutomatedHostedService>();

        builder.Services.AddHomeAssistantMqtt();

        builder.WebHost.UseWebRoot("wwwroot");
        builder.WebHost.UseStaticWebAssets();
        var app = builder.Build();

        var pathBase = GetPathBase();
        if (pathBase != null)
        {
            app.UsePathBase(pathBase);
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapControllers();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }

    private static string? GetPathBase()
    {
        if (Environment.GetEnvironmentVariable("SUPERVISOR_TOKEN") == null)
        {
            return null;
        }
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Environment.GetEnvironmentVariable("SUPERVISOR_TOKEN")! ?? "invalid");

            var response = httpClient.GetFromJsonAsync<SupoervisorResponse<AddonInfo>>("http://supervisor/addons/self/info").Result;
            if (response?.Result != "ok")
            {
                return null;
            }
            return response.Data.IngressEntry;
        }
        catch
        {
            return null;
        }

    }
}