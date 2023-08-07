using HomeAssistantAddOn.Core;

namespace Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddHomeAssistantAddOnConfig(this IConfigurationBuilder builder)
    {
        return builder.AddJsonFile($"{Utility.GetBaseDataDirectory()}options.json", optional: true);
    }
}