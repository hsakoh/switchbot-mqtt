using SwitchBotMqttApp.Models.Enums;

namespace SwitchBotMqttApp.Models.DeviceDefinitions;

public class DeviceDefinition
{
    public DeviceType DeviceType { get; set; } = default!;
    public string ApiDeviceTypeString { get; set; } = default!;
    public string? CustomizedDeviceTypeString { get; set; } = default!;
    public string? WebhookDeviceTypeString { get; set; } = default!;
    public PhysicalOrVirtual PhysicalOrVirtual { get; set; } = default!;
    public bool IsSupportPolling { get; set; } = default!;
    public bool IsSupportWebhook { get; set; } = default!;
    public bool IsSupportCommand { get; set; } = default!;
    public string? Description { get; set; } = default!;
    public string? DisplayName { get; set; } = default!;

    public FieldDefinition[] Fields { get; set; } = default!;
    public CommandDefinition[] Commands { get; set; } = default!;
}
