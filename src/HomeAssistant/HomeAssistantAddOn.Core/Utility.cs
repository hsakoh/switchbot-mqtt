namespace HomeAssistantAddOn.Core;

/// <summary>
/// Utility methods for Home Assistant Add-on integration.
/// </summary>
public static class Utility
{
    /// <summary>
    /// Gets the base data directory for persistent storage.
    /// Returns /data/ for Home Assistant Add-on environment, or custom path if OVERRIDE_DATA_PATH is set.
    /// </summary>
    /// <returns>The base data directory path.</returns>
    public static string GetBaseDataDirectory() 
    {
        if (Environment.GetEnvironmentVariables().Contains("OVERRIDE_DATA_PATH"))
        {
            return Environment.GetEnvironmentVariable("OVERRIDE_DATA_PATH")!;
        }
        return "/data/";
    }
}
