using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwitchBotMqttApp.Logics;
using SwitchBotMqttApp.Models.DeviceConfiguration;
using SwitchBotMqttApp.Models.Enums;
using SwitchBotMqttApp.Pages.Modal;
using System.Text.Json;

namespace SwitchBotMqttApp.Pages;

public partial class DeviceConfiguration : ComponentBase
{
    [CascadingParameter] 
    public IModalService Modal { get; set; } = default!;

    [Inject]
    protected DeviceConfigurationManager DeviceConfigurationManager { get; set; } = default!;
    [Inject]
    protected DeviceDefinitionsManager DeviceDefinitionsManager { get; set; } = default!;
    [Inject]
    protected SwitchBotApiClient SwitchBotApiClient { get; set; } = default!;
    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Model.Data = new DevicesConfig();
        Model.Data = await DeviceConfigurationManager.GetFileAsync();
    }

    private readonly DeviceConfigurationModel Model = new();


    public async Task FetchSwitchBotApi()
    {
        Model.Fetching = true;
        StateHasChanged();
        await DeviceConfigurationManager.LoadDevicesAsync(Model.Data);
        Model.Fetching = false;
    }
    public async Task Save()
    {
        Model.Saveing = true;

        foreach (var deleteDevice in Model.Data.PhysicalDevices
            .Where(d => d.ConfigureStatus == ConfigureStatus.Missing || d.ConfigureStatus == ConfigureStatus.Deleting).ToArray())
        {
            Model.Data.PhysicalDevices.Remove(deleteDevice);
        }
        foreach (var deleteDevice in Model.Data.VirtualInfraredRemoteDevices
            .Where(d => d.ConfigureStatus == ConfigureStatus.Missing || d.ConfigureStatus == ConfigureStatus.Deleting).ToArray())
        {
            Model.Data.VirtualInfraredRemoteDevices.Remove(deleteDevice);
        }
        StateHasChanged();
        await DeviceConfigurationManager.SaveFileAsync(Model.Data);
        Model.Saveing = false;
    }
    public async Task Restore()
    {
        Model.Saveing = true;
        StateHasChanged();
        Model.Data = await DeviceConfigurationManager.GetFileAsync();
        Model.Saveing = false;
    }
    public void DeleteDevice(PhysicalDevice physicalDevice)
    {
        if (physicalDevice.ConfigureStatus == ConfigureStatus.Missing
            || physicalDevice.ConfigureStatus == ConfigureStatus.Deleting)
        {
            physicalDevice.ConfigureStatus = ConfigureStatus.NoChange;
        }
        else
        {
            physicalDevice.ConfigureStatus = ConfigureStatus.Deleting;
        }
    }
    public void DeleteDevice(VirtualInfraredRemoteDevice remoteDevice)
    {
        if (remoteDevice.ConfigureStatus == ConfigureStatus.Missing
            || remoteDevice.ConfigureStatus == ConfigureStatus.Deleting)
        {
            remoteDevice.ConfigureStatus = ConfigureStatus.NoChange;
        }
        else
        {
            remoteDevice.ConfigureStatus = ConfigureStatus.Deleting;
        }
    }
    public void DeleteCustomCommand(DeviceBase device, CommandConfig command)
    {
        device.Commands.Remove(command);
    }
    public void AddCustomizeCommand(DeviceBase device)
    {
        device.Commands.Add(new CommandConfig()
        {
            CommandType = CommandType.Customize,
            Command = Model.AddingCustomCommand[device].CommandCustomize,
            DisplayName = Model.AddingCustomCommand[device].DisplayNameCustomize,
            Enable = true,
        });
    }
    public void AddTagCommand(DeviceBase device)
    {
        device.Commands.Add(new CommandConfig()
        {
            CommandType = CommandType.Tag,
            Command = string.IsNullOrEmpty(Model.AddingCustomCommand[device].CommandTag)?
                Model.AddingCustomCommand[device].CommandTagTextInput
                : Model.AddingCustomCommand[device].CommandTag,
            DisplayName = Model.AddingCustomCommand[device].DisplayNameTag,
            Enable = true,
        });
    }

    public void ExecuteDefaultCommand(DeviceBase device,CommandConfig command)
    {
        var parameters = new ModalParameters
        {
            { nameof(CommandTestModal.Device), device },
            { nameof(CommandTestModal.CommandConf), command }
        };
        var options = new ModalOptions()
        {
            Size = ModalSize.ExtraLarge
        };

        Modal.Show<CommandTestModal>("Command Test", parameters, options);
    }

    public void ShowDeviceAttribute(DeviceBase device)
    {
        var parameters = new ModalParameters
        {
            { nameof(DeviceAttributeModal.Response), device.RawValue!.ToJsonString(JsonSerializerOptions) }
        };
        var options = new ModalOptions()
        {
            Size = ModalSize.Medium
        };

        Modal.Show<DeviceAttributeModal>("Device Attribute", parameters, options);
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
    };
}

public class DeviceConfigurationModel
{
    public DevicesConfig Data { get; set; } = default!;

    public bool Fetching { get; set; } = false;

    public bool Saveing { get; set; } = false;

    public int EstimateApiCallPerDay => (int)Data.PhysicalDevices.Where(s => s.UsePolling == true).Sum(s => TimeSpan.FromDays(1) / s.PollingInterval);

    private readonly Dictionary<DeviceBase, CustomCommand> addingCustomCommand = new();
    public Dictionary<DeviceBase, CustomCommand> AddingCustomCommand
    {
        get
        {
            Data.VirtualInfraredRemoteDevices.ForEach(d =>
            {
                if (!addingCustomCommand.ContainsKey(d))
                {
                    addingCustomCommand.Add(d, new CustomCommand());
                }
            });
            return addingCustomCommand;
        }
    }

    public class CustomCommand
    {
        public string CommandCustomize { get; set; } = default!;
        public string CommandTag { get; set; } = default!;
        public string CommandTagTextInput { get; set; } = default!;
        public string DisplayNameCustomize { get; set; } = default!;
        public string DisplayNameTag { get; set; } = default!;
    }
}