using Microsoft.Extensions.Logging;

namespace HomeAssistantAddOn.Core;

public class CommonOptions
{
    /// <summary>
    /// Trace,Debug,Information,Warning,Error,Critical,None
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}
