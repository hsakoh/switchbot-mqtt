using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum RemoteDeviceType
{
    /// <summary>
    /// Unknown device type.
    /// </summary>
    [EnumMember(Value = "Unknown")]
    Unknown,
    /// <summary>
    ///  Air Conditioner remote device type.
    /// </summary>
    [EnumMember(Value = "Air Conditioner")]
    AirConditioner,
    /// <summary>
    ///  Air Purifier remote device type.
    /// </summary>
    [EnumMember(Value = "Air Purifier")]
    AirPurifier,
    /// <summary>
    ///  Camera remote device type.
    /// </summary>
    [EnumMember(Value = "Camera")]
    Camera,
    /// <summary>
    ///  DVD remote device type.
    /// </summary>
    [EnumMember(Value = "DVD")]
    DVD,
    /// <summary>
    ///  Fan remote device type.
    /// </summary>
    [EnumMember(Value = "Fan")]
    Fan,
    /// <summary>
    ///  IPTV remote device type.
    /// </summary>
    [EnumMember(Value = "IPTV")]
    IPTV,
    /// <summary>
    ///  Light remote device type.
    /// </summary>
    [EnumMember(Value = "Light")]
    Light,
    /// <summary>
    ///  Others remote device type.
    /// </summary>
    [EnumMember(Value = "Others")]
    Others,
    /// <summary>
    ///  Projector remote device type.
    /// </summary>
    [EnumMember(Value = "Projector")]
    Projector,
    /// <summary>
    ///  Set Top Box remote device type.
    /// </summary>
    [EnumMember(Value = "Set Top Box")]
    SetTopBox,
    /// <summary>
    ///  Streamer remote device type.
    /// </summary>
    [EnumMember(Value = "Streamer")]
    Streamer,
    /// <summary>
    ///  Speaker remote device type.
    /// </summary>
    [EnumMember(Value = "Speaker")]
    Speaker,
    /// <summary>
    ///  TV remote device type.
    /// </summary>
    [EnumMember(Value = "TV")]
    TV,
    /// <summary>
    ///  Vacuum Cleaner remote device type.
    /// </summary>
    [EnumMember(Value = "Vacuum Cleaner")]
    VacuumCleaner,
    /// <summary>
    ///  Water Heater remote device type.
    /// </summary>
    [EnumMember(Value = "Water Heater")]
    WaterHeater,

}