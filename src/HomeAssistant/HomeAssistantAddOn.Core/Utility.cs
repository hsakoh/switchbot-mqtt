namespace HomeAssistantAddOn.Core;

public static class Utility
{
    public static string GetBaseDataDirectory() 
    {
        if (Environment.GetEnvironmentVariables().Contains("OVERRIDE_DATA_PATH"))
        {
            return Environment.GetEnvironmentVariable("OVERRIDE_DATA_PATH")!;
        }
        return "/data/";
    }
}
