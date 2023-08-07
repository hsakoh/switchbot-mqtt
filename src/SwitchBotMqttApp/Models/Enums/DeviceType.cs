using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.Enums;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DeviceType
{
    //PhysicalDevice
    Bot,
    Curtain,
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
    CeilingLight,
    CeilingLightPro,
    PlugMiniUs,
    PlugMiniJp,
    Plug,
    StripLight,
    ColorBulb,
    RobotVacuumCleanerS1,
    RobotVacuumCleanerS1Plus,
    Humidifier,
    IndoorCam,
    PanTiltCam,
    BlindTilt,

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