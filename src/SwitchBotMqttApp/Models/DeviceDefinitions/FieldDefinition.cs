using SwitchBotMqttApp.Models.Enums;
using SwitchBotMqttApp.Models.HomeAssistant;
using SwitchBotMqttApp.Models.Mqtt;

namespace SwitchBotMqttApp.Models.DeviceDefinitions;

public class FieldDefinition
{
    public DeviceType DeviceType { get; set; } = default!;
    public string FieldName { get; set; } = default!;
    public FieldSourceType FieldSourceType { get; set; } = default!;
    public string? StatusKey { get; set; } = default!;
    public string? WebhookKey { get; set; } = default!;
    public FieldDataType FieldDataType { get; set; } = default!;
    public string? Description { get; set; } = default!;
    public bool IsBinary { get; set; } = default!;
    public BinarySensorDeviceClass? BinarySensorDeviceClass { get; set; } = default!;
    public string? OnValue { get; set; } = default!;
    public string? OffValue { get; set; } = default!;
    public string? Icon { get; set; } = default!;
    public SensorDeviceClass? SensorDeviceClass { get; set; } = default!;
    public string? EntityCategory { get; set; } = default!;
    public string? StateClass { get; set; } = default!;
    public string? UnitOfMeasurement { get; set; } = default!;
    public string? DisplayName { get; set; } = default!;
    public string? DisplayNameJa { get; set; } = default!;
}
