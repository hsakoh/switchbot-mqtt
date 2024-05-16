using SwitchBotMqttApp.Models.Enums;
using SwitchBotMqttApp.Models.HomeAssistant;

namespace SwitchBotMqttApp.Models.DeviceDefinitions;

public class CommandPayloadDefinition
{
    public DeviceType DeviceType { get; set; } = default!;
    public CommandType CommandType { get; set; } = default!;
    public string Command { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Path { get; set; } = default!;
    public int Index { get; set; } = default!;

    public string Description { get; set; } = default!;
    public ParameterType ParameterType { get; set; } = default!;
    public string? Icon { get; set; } = default!;
    public string? Options { get; set; } = default!;
    public string? OptionsDescription { get; set; } = default!;
    public long? RangeMin { get; set; } = default!;
    public long? RangeMax { get; set; } = default!;
    public NumberDeviceClass? NumberDeviceClass { get; set; } = default!;
    public NumberMode? NumberMode { get; set; } = default!;
    public string? UnitOfMeasurement { get; set; } = default!;
    public int? LengthMin { get; set; } = default!;
    public int? LengthMax { get; set; } = default!;
    public string? DefaultValue { get; set; } = default!;
    public string? DisplayName { get; set; } = default!;
    public string? DisplayNameJa { get; set; } = default!;

    public string[]? GetOptions()
    {
        return Options?.Split('|');
    }

    public string[]? GetOptionsDescription()
    {
        return OptionsDescription?.Split('|');
    }

    public string OptionToDescription(string? option)
    {
        var index = Array.IndexOf(GetOptions()!, option);
        if (index == -1)
        {
            return string.Empty;
        }
        return GetOptionsDescription()![index];
    }

    public string DescriptionToOption(string? description)
    {
        var index = Array.IndexOf(GetOptionsDescription()!, description);
        if(index == -1)
        {
            return string.Empty;
        }
        return GetOptions()![index];
    }

}
