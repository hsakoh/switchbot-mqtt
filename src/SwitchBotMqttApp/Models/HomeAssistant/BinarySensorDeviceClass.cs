using System.Runtime.Serialization;

namespace SwitchBotMqttApp.Models.HomeAssistant;
public enum BinarySensorDeviceClass
{
    /// <summary>
    /// On means low, Off means normal.
    /// </summary>
    [EnumMember(Value = "battery")]
    Battery,
    /// <summary>
    /// On means charging, Off means not charging.
    /// </summary>
    [EnumMember(Value = "battery_charging")]
    BatteryCharging,
    /// <summary>
    /// On means carbon monoxide detected, Off means no carbon monoxide (clear).
    /// </summary>
    [EnumMember(Value = "co")]
    Co,
    /// <summary>
    /// On means cold, Off means normal.
    /// </summary>
    [EnumMember(Value = "cold")]
    Cold,
    /// <summary>
    /// On means connected, Off means disconnected.
    /// </summary>
    [EnumMember(Value = "connectivity")]
    Connectivity,
    /// <summary>
    /// On means open, Off means closed.
    /// </summary>
    [EnumMember(Value = "door")]
    Door,
    /// <summary>
    /// On means open, Off means closed.
    /// </summary>
    [EnumMember(Value = "garage_door")]
    Garage_door,
    /// <summary>
    /// On means gas detected, Off means no gas (clear).
    /// </summary>
    [EnumMember(Value = "gas")]
    Gas,
    /// <summary>
    /// On means hot, Off means normal.
    /// </summary>
    [EnumMember(Value = "heat")]
    Heat,
    /// <summary>
    /// On means light detected, Off means no light.
    /// </summary>
    [EnumMember(Value = "light")]
    Light,
    /// <summary>
    /// On means open (unlocked), Off means closed (locked).
    /// </summary>
    [EnumMember(Value = "lock")]
    Lock,
    /// <summary>
    /// On means wet, Off means dry.
    /// </summary>
    [EnumMember(Value = "moisture")]
    Moisture,
    /// <summary>
    /// On means motion detected, Off means no motion (clear).
    /// </summary>
    [EnumMember(Value = "motion")]
    Motion,
    /// <summary>
    /// On means moving, Off means not moving (stopped).
    /// </summary>
    [EnumMember(Value = "moving")]
    Moving,
    /// <summary>
    /// On means occupied, Off means not occupied (clear).
    /// </summary>
    [EnumMember(Value = "occupancy")]
    Occupancy,
    /// <summary>
    /// On means open, Off means closed.
    /// </summary>
    [EnumMember(Value = "opening")]
    Opening,
    /// <summary>
    /// On means plugged in, Off means unplugged.
    /// </summary>
    [EnumMember(Value = "plug")]
    Plug,
    /// <summary>
    /// On means power detected, Off means no power.
    /// </summary>
    [EnumMember(Value = "power")]
    Power,
    /// <summary>
    /// On means home, Off means away.
    /// </summary>
    [EnumMember(Value = "presence")]
    Presence,
    /// <summary>
    /// On means problem detected, Off means no problem (OK).
    /// </summary>
    [EnumMember(Value = "problem")]
    Problem,
    /// <summary>
    /// On means running, Off means not running.
    /// </summary>
    [EnumMember(Value = "running")]
    Running,
    /// <summary>
    /// On means unsafe, Off means safe.
    /// </summary>
    [EnumMember(Value = "safety")]
    Safety,
    /// <summary>
    /// On means smoke detected, Off means no smoke (clear).
    /// </summary>
    [EnumMember(Value = "smoke")]
    Smoke,
    /// <summary>
    /// On means sound detected, Off means no sound (clear).
    /// </summary>
    [EnumMember(Value = "sound")]
    Sound,
    /// <summary>
    /// On means tampering detected, Off means no tampering (clear)
    /// </summary>
    [EnumMember(Value = "tamper")]
    Tamper,
    /// <summary>
    /// On means update available, Off means up-to-date. The use of this device class should be avoided, please consider using the update entity instead.
    /// </summary>
    [EnumMember(Value = "update")]
    Update,
    /// <summary>
    /// On means vibration detected, Off means no vibration.
    /// </summary>
    [EnumMember(Value = "vibration")]
    Vibration,
    /// <summary>
    /// On means open, Off means closed.
    /// </summary>
    [EnumMember(Value = "window")]
    Window,
}