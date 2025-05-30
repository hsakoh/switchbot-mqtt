using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.Enums;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DeviceType
{
    //PhysicalDevice
    Bot,
    Curtain,
    Curtain3,
    RollerShade,
    Hub,
    HubPlus,
    HubMini,
    Hub2,
    Hub3,
    Meter,
    MeterPlus,
    OutdoorMeter,
    MeterPro,
    MeterProCO2,
    Lock,
    LockPro,
    LockLite,
    LockUltra,
    Keypad,
    KeypadTouch,
    KeypadVision,
    Remote,
    MotionSensor,
    ContactSensor,
    WaterDetector,
    CeilingLight,
    CeilingLightPro,
    PlugMiniUs,
    PlugMiniJp,
    Plug,
    RelaySwitch1PM,
    RelaySwitch1,
    RelaySwitch2PM,
    GarageDoorOpener,
    StripLight,
    ColorBulb,
    StripLight3,
    FloorLamp,
    RobotVacuumCleanerS1,
    RobotVacuumCleanerS1Plus,
    MiniRobotVacuumK10Plus,
    MiniRobotVacuumK10PlusPro,
    RobotVacuumCleanerS10,
    RobotVacuumCleanerS20,
    RobotVacuumCleanerK10PlusProCombo,
    RobotVacuumCleanerK20PlusPro,
    Humidifier,
    EvaporativeHumidifier,
    AirPurifierPM25,
    AirPurifierTablePM25,
    AirPurifierVOC,
    AirPurifierTableVOC,
    IndoorCam,
    PanTiltCam,
    PanTiltCam2K,
    BlindTilt,
    BatteryCirculatorFan,
    CirculatorFan,
    UniversalRemote,
    PanTiltCamPlus5mp,
    OutdoorSpotlightCam2K,
    WalletFinderCard,
    VideoDoorbell,

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