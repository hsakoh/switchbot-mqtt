using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.SwitchBotApi;

public class GetDevicesResponseBody
{
    [JsonPropertyName("deviceList")]
    public Devicelist[] DeviceList { get; set; } = default!;
    [JsonPropertyName("infraredRemoteList")]
    public Infraredremotelist[] InfraredRemoteList { get; set; } = default!;
    public class Devicelist
    {
        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; } = default!;
        [JsonPropertyName("deviceName")]
        public string DeviceName { get; set; } = default!;
        [JsonPropertyName("deviceType")]
        public string DeviceType { get; set; } = default!;
        [JsonPropertyName("enableCloudService")]
        public bool EnableCloudService { get; set; } = default!;
        [JsonPropertyName("hubDeviceId")]
        public string HubDeviceId { get; set; } = default!;

        //BlindTilt,Curtain,Roller Shade

        [JsonPropertyName("curtainDevicesIds")]
        public string[]? CurtainDevicesIds { get; set; }
        [JsonPropertyName("blindTiltDevicesIds")]
        public string[]? BlindTiltDevicesIds { get; set; }
        [JsonPropertyName("groupingDevicesIds")]
        public string[]? GroupingDevicesIds { get; set; }
        [JsonPropertyName("calibrate")]
        public bool? Calibrate { get; set; }
        [JsonPropertyName("group")]
        public bool? Group { get; set; }
        [JsonPropertyName("master")]
        public bool? Master { get; set; }
        [JsonPropertyName("direction")]
        public string? Direction { get; set; }
        [JsonPropertyName("slidePosition")]
        public int SlidePosition { get; set; }
        [JsonPropertyName("openDirection")]
        public string? OpenDirection { get; set; }
        [JsonPropertyName("version")]
        public int Version { get; set; }
        [JsonPropertyName("bleVersion")]
        public int BleVersion { get; set; }

        // Lock 
        [JsonPropertyName("groupName")]
        public string? GroupName { get; set; }
        [JsonPropertyName("lockDevicesIds")]
        public string[]? LockDevicesIds { get; set; }
        // & group,master

        //Keypad,Keypad Touch
        [JsonPropertyName("lockDeviceId")]
        public string LockDeviceId { get; set; } = default!;
        [JsonPropertyName("keyList")]
        public KeyListItem[] KeyList { get; set; } = default!;


        public class KeyListItem
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; } = default!;

            [JsonPropertyName("type")]
            public string Type { get; set; } = default!;

            [JsonPropertyName("password")]
            public string Password { get; set; } = default!;

            [JsonPropertyName("iv")]
            public string Iv { get; set; } = default!;

            [JsonPropertyName("status")]
            public string Status { get; set; } = default!;

            [JsonPropertyName("createTime")]
            public long CreateTime { get; set; }
        }
    }

    public class Infraredremotelist
    {
        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; } = default!;
        [JsonPropertyName("deviceName")]
        public string DeviceName { get; set; } = default!;
        [JsonPropertyName("remoteType")]
        public string RemoteType { get; set; } = default!;
        [JsonPropertyName("hubDeviceId")]
        public string HubDeviceId { get; set; } = default!;
    }
}
