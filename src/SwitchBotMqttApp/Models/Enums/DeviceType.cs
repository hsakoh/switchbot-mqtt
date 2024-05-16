using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.Enums;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DeviceType
{
    //PhysicalDevice
    Bot,
    Curtain,
    Curtain3,
    Hub,
    HubPlus,
    HubMini,
    Hub2,
    Meter,
    MeterPlus,
    OutdoorMeter,
    Lock,
    Keypad,
    KeypadTouch,
    Remote,
    MotionSensor,
    ContactSensor,
    WaterDetector,
    CeilingLight,
    CeilingLightPro,
    PlugMiniUs,
    PlugMiniJp,
    Plug,
    StripLight,
    ColorBulb,
    RobotVacuumCleanerS1,
    RobotVacuumCleanerS1Plus,
    RobotVacuumCleanerS10,
    Humidifier,
    IndoorCam,
    PanTiltCam,
    BlindTilt,
    BatteryCirculatorFan,

    //VirtualInfraredRemoteDevice
    AirConditioner,
    TV,
    IPTV,
    SetTopBox,
    DVD,
    Fan,
    Projector,
    Camera,
    Others,
    AirPurifier,
    Speaker,
    WaterHeater,
    VacuumCleaner,
    Light,
}