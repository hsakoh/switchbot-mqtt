![logo](_build_on_haos/logo.svg)

# SwitchBot MQTT Home Assistant App/Add-on

This project is a Home Assistant app that allows you to control various SwitchBot products through the API.

The app can also receive Webhooks to obtain the device's status.
Via an MQTT broker, it will be detected as an MQTT integration in Home Assistant.

You can perform manual scene executions that were configured in the SwitchBot app, as well as control virtual infrared remote devices.

**Important: Please note that this app does not support operations on SwitchBot devices via Bluetooth.**

**New custom integration for Pan/Tilt Cam Plus [here](https://github.com/hsakoh/ha-switchbot-kvs-camera).**

## Current Support Status

![aarch64-shield](https://img.shields.io/badge/aarch64-yes-green.svg)
![amd64-shield](https://img.shields.io/badge/amd64-yes-green.svg)
![armv7-shield](https://img.shields.io/badge/armv7-yes-green.svg)

[![Open your Home Assistant instance and show the add add-on repository dialog with a specific repository URL pre-filled.](https://my.home-assistant.io/badges/supervisor_add_addon_repository.svg)](https://my.home-assistant.io/redirect/supervisor_add_addon_repository/?repository_url=https%3A%2F%2Fgithub.com%2Fhsakoh%2Fha-addon)

### Physical Devices

We have implemented all devices according to the published API specifications, but testing has been conducted only on a subset.

- The "-" in the "Verification" column indicates that product was tested and found to have no specific functionality.
- To check which values can be referenced and what operations can be performed for each device, please refer to the links provided to each checkbox.
- Even for products not officially documented, the API may indicate the device type as another product. Additionally, devices with similar fields and commands may function if their device type is spoofed. For example, recognizing a K10+ as an S1 may enable operation. Configure the `EnforceDeviceTypes` option in your settings.

| Device                                                                                                                             | [OpenAPI v1.1<br>Documented][GetDeviceList] |         [Status<br>API][StatusAPI]          |              [Webhook][Webhook]              |         [Command<br>API][CommandAPI]         | Verification |
| ---------------------------------------------------------------------------------------------------------------------------------- | :-----------------------------------------: | :-----------------------------------------: | :------------------------------------------: | :------------------------------------------: | :----------: |
| **Hub**                                                                                                                            |                      -                      |                      -                      |                      -                       |                      -                       |      -       |
| Hub                                                                                                                                |                [✅][HubList]                 |                      -                      |                      -                       |                      -                       |              |
| Hub Plus                                                                                                                           |              [✅][HubPlusList]               |                      -                      |                      -                       |                      -                       |              |
| [Hub Mini][HubMiniProduct] [[JP][HubMiniProductJP]]                                                                                |              [✅][HubMiniList]               |                      -                      |                      -                       |                      -                       |      -       |
| [Hub 2][Hub2Product] [[JP][Hub2ProductJP]]                                                                                         |                [✅][Hub2List]                |               [✅][Hub2Status]               |               [✅][Hub2Webhook]               |                      -                       |      ✅       |
| [Hub 3][Hub3Product] [[JP][Hub3ProductJP]]                                                                                         |                [✅][Hub3List]                |               [✅][Hub3Status]               |               [✅][Hub3Webhook]               |                      -                       |      ✅       |
| [AI Hub](AIHubProduct) [[JP][AIHubProductJP]]                                                                                      |               [✅][AIHubList]                |              [✅][AIHubStatus]               |                      -                       |                      -                       |              |
| **Home Automation**                                                                                                                |                      -                      |                      -                      |                      -                       |                      -                       |      -       |
| [Bot][BotProduct] [[JP][BotProductJP]]                                                                                             |                [✅][BotList]                 |               [✅][BotStatus]                |               [✅][BotWebhook]                |               [✅][BotCommand]                |              |
| [Curtain][CurtainProduct]                                                                                                          |              [✅][CurtainList]               |             [✅][CurtainStatus]              |             [✅][CurtainWebhook]              |             [✅][CurtainCommand]              |      ✅       |
| [Curtain3][Curtain3Product] [[JP][Curtain3ProductJP]]                                                                              |              [✅][Curtain3List]              |             [✅][Curtain3Status]             |             [✅][Curtain3Webhook]             |             [✅][Curtain3Command]             |              |
| [Blind Tilt][BlindTiltProduct] [[JP][BlindTiltProductJP]]                                                                          |             [✅][BlindTiltList]              |            [✅][BlindTiltStatus]             |                      -                       |            [✅][BlindTiltCommand]             |              |
| [Roller Shade][RollerShadeProduct]                                                                                                 |            [✅][RollerShadeList]             |           [✅][RollerShadeStatus]            |           [✅][RollerShadeWebhook]            |           [✅][RollerShadeCommand]            |      ✅       |
| [Universal Remote][UniversalRemoteProduct] [[JP][UniversalRemoteProductJP]]                                                        |                      -                      |                      ✅                      |                      -                       |                      -                       |      ✅       |
| [Wallet Finder Card][WalletFinderCardProduct] [[JP][WalletFinderCardProductJP]]                                                    |                      -                      |                      -                      |                      -                       |                      -                       |      -       |
| [Relay Switch 1PM][RelaySwitch1PMProduct]                                                                                          |           [✅][RelaySwitch1PMList]           |          [✅][RelaySwitch1PMStatus]          |          [✅][RelaySwitch1PMWebhook]          |          [✅][RelaySwitch1PMCommand]          |      ✅       |
| [Relay Switch 1][RelaySwitch1Product] [[JP][RelaySwitch1ProductJP]]                                                                |            [✅][RelaySwitch1List]            |           [✅][RelaySwitch1Status]           |           [✅][RelaySwitch1Webhook]           |           [✅][RelaySwitch1Command]           |      ✅       |
| [Relay Switch 2PM][RelaySwitch2PMProduct]                                                                                          |           [✅][RelaySwitch2PMList]           |          [✅][RelaySwitch2PMStatus]          |          [✅][RelaySwitch2PMWebhook]          |          [✅][RelaySwitch2PMCommand]          |      ✅       |
| [Garage Door Opener][GarageDoorOpenerProduct]                                                                                      |          [✅][GarageDoorOpenerList]          |         [✅][GarageDoorOpenerStatus]         |         [✅][GarageDoorOpenerWebhook]         |         [✅][GarageDoorOpenerCommand]         |              |
| [Smart Radiator Thermostat][SmartRadiatorThermostatProduct]                                                                        |      [✅][SmartRadiatorThermostatList]       |     [✅][SmartRadiatorThermostatStatus]      |     [✅][SmartRadiatorThermostatWebhook]      |     [✅][SmartRadiatorThermostatCommand]      |              |
| [Home Climate Panel][ClimatePanelProduct] [[JP][ClimatePanelProductJP]]                                                            |            [✅][ClimatePanelList]            |           [✅][ClimatePanelStatus]           |           [✅][ClimatePanelWebhook]           |                      -                       |              |
| [Kata Friends][KataFriendsProduct] [[JP][KataFriendsProductJP]]                                                                    |            [✅][KataFriendsList]             |           [✅][KataFriendsStatus]            |           [✅][KataFriendsWebhook]            |           [✅][KataFriendsCommand]            |              |
| AI MindClip                                                                                                                        |             [✅][AIMindClipList]             |            [✅][AIMindClipStatus]            |            [✅][AIMindClipWebhook]            |                      -                       |              |
| **Home Appliance**                                                                                                                 |                      -                      |                      -                      |                      -                       |                      -                       |      -       |
| Humidifier [[JP][HumidifierProductJP]]                                                                                             |             [✅][HumidifierList]             |            [✅][HumidifierStatus]            |                      -                       |            [✅][HumidifierCommand]            |              |
| Evaporative Humidifier [[JP][EvaporativeHumidifierProductJP]]                                                                      |       [✅][EvaporativeHumidifierList]        |      [✅][EvaporativeHumidifierStatus]       |      [✅][EvaporativeHumidifierWebhook]       |      [✅][EvaporativeHumidifierCommand]       |              |
| [Evaporative Humidifier Auto-refill][EvaporativeHumidifierAutoRefillProduct] [[JP][EvaporativeHumidifierAutoRefillProductJP]]      |  [✅][EvaporativeHumidifierAutoRefillList]   | [✅][EvaporativeHumidifierAutoRefillStatus]  | [✅][EvaporativeHumidifierAutoRefillWebhook]  | [✅][EvaporativeHumidifierAutoRefillCommand]  |              |
| Fan                                                                                                                                |                      -                      |                                             |                                              |                                              |              |
| [Battery Circulator Fan][BatteryCirculatorFanProduct] [[JP][BatteryCirculatorFanProductJP]]                                        |        [✅][BatteryCirculatorFanList]        |       [✅][BatteryCirculatorFanStatus]       |       [✅][BatteryCirculatorFanWebhook]       |       [✅][BatteryCirculatorFanCommand]       |              |
| Battery Circulator Fan 2 Pro [[JP][BatteryCirculatorFan2ProProductJP]]                                                             |     [✅][BatteryCirculatorFan2ProList]      |    [✅][BatteryCirculatorFan2ProStatus]     |    [✅][BatteryCirculatorFan2ProWebhook]     |    [✅][BatteryCirculatorFan2ProCommand]     |              |
| [Circulator Fan][CirculatorFanProduct] [[JP][CirculatorFanProductJP]]                                                              |           [✅][CirculatorFanList]            |          [✅][CirculatorFanStatus]           |          [✅][CirculatorFanWebhook]           |          [✅][CirculatorFanCommand]           |      ✅       |
| Standing Circulator Fan [JP]                                                                                                       |       [✅][StandingCirculatorFanList]        |      [✅][StandingCirculatorFanStatus]       |      [✅][StandingCirculatorFanWebhook]       |      [✅][StandingCirculatorFanCommand]       |              |
| [Air Purifier PM2.5][AirPurifierPM25Product]                                                                                       |          [✅][AirPurifierPM25List]           |         [✅][AirPurifierPM25Status]          |         [✅][AirPurifierPM25Webhook]          |         [✅][AirPurifierPM25Command]          |              |
| [Air Purifier Table PM2.5][AirPurifierTablePM25Product]                                                                            |        [✅][AirPurifierTablePM25List]        |       [✅][AirPurifierTablePM25Status]       |       [✅][AirPurifierTablePM25Webhook]       |       [✅][AirPurifierTablePM25Command]       |              |
| Air Purifier VOC [[JP][AirPurifierVOCProductJP]]                                                                                   |           [✅][AirPurifierVOCList]           |          [✅][AirPurifierVOCStatus]          |          [✅][AirPurifierVOCWebhook]          |          [✅][AirPurifierVOCCommand]          |              |
| Air Purifier Table VOC [[JP][AirPurifierTableVOCProductJP]]                                                                        |        [✅][AirPurifierTableVOCList]         |       [✅][AirPurifierTableVOCStatus]        |       [✅][AirPurifierTableVOCWebhook]        |       [✅][AirPurifierTableVOCCommand]        |              |
| [AI Art Frame][AIArtFrameProduct] [[JP][AIArtFrameProductJP]]                                                                      |             [✅][AIArtFrameList]             |            [✅][AIArtFrameStatus]            |            [✅][AIArtFrameWebhook]            |            [✅][AIArtFrameCommand]            |              |
| **Robot Vacuum**                                                                                                                   |                      -                      |                      -                      |                      -                       |                      -                       |      -       |
| Robot Vacuum Cleaner S1 [[JP][RobotVacuumCleanerS1ProductJP]]                                                                      |        [✅][RobotVacuumCleanerS1List]        |       [✅][RobotVacuumCleanerS1Status]       |       [✅][RobotVacuumCleanerS1Webhook]       |       [✅][RobotVacuumCleanerS1Command]       |              |
| Robot Vacuum Cleaner S1 Plus [[JP][RobotVacuumCleanerS1PlusProductJP]]                                                             |      [✅][RobotVacuumCleanerS1PlusList]      |     [✅][RobotVacuumCleanerS1PlusStatus]     |     [✅][RobotVacuumCleanerS1PlusWebhook]     |     [✅][RobotVacuumCleanerS1PlusCommand]     |              |
| [Mini Robot Vacuum K10+][MiniRobotVacuumK10+Product] [[JP][MiniRobotVacuumK10+ProductJP]]                                          |        [✅][MiniRobotVacuumK10+List]         |       [✅][MiniRobotVacuumK10+Status]        |       [✅][MiniRobotVacuumK10+Webhook]        |       [✅][MiniRobotVacuumK10+Command]        |              |
| [Mini Robot Vacuum K10+ Pro][MiniRobotVacuumK10+ProProduct] [[JP][MiniRobotVacuumK10+ProProductJP]]                                |       [✅][MiniRobotVacuumK10+ProList]       |      [✅][MiniRobotVacuumK10+ProStatus]      |      [✅][MiniRobotVacuumK10+ProWebhook]      |      [✅][MiniRobotVacuumK10+ProCommand]      |      ✅       |
| [Multitasking Household Robot K20+ Pro][MultitaskingHouseholdRobotK20ProProduct] [[JP][MultitaskingHouseholdRobotK20ProProductJP]] |  [✅][MultitaskingHouseholdRobotK20ProList]  | [✅][MultitaskingHouseholdRobotK20ProStatus] | [✅][MultitaskingHouseholdRobotK20ProWebhook] | [✅][MultitaskingHouseholdRobotK20ProCommand] |              |
| [Floor Cleaning Robot S10][FloorCleaningRobotS10Product] [[JP][FloorCleaningRobotS10ProductJP]]                                    |       [✅][FloorCleaningRobotS10List]        |      [✅][FloorCleaningRobotS10Status]       |      [✅][FloorCleaningRobotS10Webhook]       |      [✅][FloorCleaningRobotS10Command]       |              |
| [Floor Cleaning Robot S20][FloorCleaningRobotS20Product] [[JP][FloorCleaningRobotS20ProductJP]]                                    |       [✅][FloorCleaningRobotS20List]        |      [✅][FloorCleaningRobotS20Status]       |      [✅][FloorCleaningRobotS20Webhook]       |      [✅][FloorCleaningRobotS20Command]       |              |
| [Robot Vacuum K10+ Pro Combo][RobotVacuumK10+ProComboProduct] [[JP][RobotVacuumK10+ProComboProductJP]]                             |      [✅][RobotVacuumK10+ProComboList]       |     [✅][RobotVacuumK10+ProComboStatus]      |     [✅][RobotVacuumK10+ProComboWebhook]      |     [✅][RobotVacuumK10+ProComboCommand]      |              |
| [Robot Vacuum K11+][RobotVacuumK11+Product] [[JP][RobotVacuumK11+ProductJP]]                                                       |          [✅][RobotVacuumK11+List]           |         [✅][RobotVacuumK11+Status]          |         [✅][RobotVacuumK11+Webhook]          |         [✅][RobotVacuumK11+Command]          |              |
| **Sensor**                                                                                                                         |                      -                      |                      -                      |                      -                       |                      -                       |      -       |
| [Meter][MeterProduct] [[JP][MeterProductJP]]                                                                                       |               [✅][MeterList]                |              [✅][MeterStatus]               |              [✅][MeterWebhook]               |                      -                       |              |
| [Meter Plus][MeterPlusProduct] [[JP][MeterPlusProductJP]]                                                                          |             [✅][MeterPlusList]              |            [✅][MeterPlusStatus]             |            [✅][MeterPlusWebhook]             |                      -                       |      ✅       |
| [Meter Pro][MeterProProduct] [[JP][MeterProProductJP]]                                                                             |              [✅][MeterProList]              |             [✅][MeterProStatus]             |             [✅][MeterProWebhook]             |                      -                       |              |
| [Meter Pro CO2][MeterProCO2Product] [[JP][MeterProCO2ProductJP]]                                                                   |            [✅][MeterProCO2List]             |           [✅][MeterProCO2Status]            |           [✅][MeterProCO2Webhook]            |                      -                       |      ✅       |
| [Outdoor Meter][OutdoorMeterProduct] [[JP][OutdoorMeterProductJP]]                                                                 |            [✅][OutdoorMeterList]            |           [✅][OutdoorMeterStatus]           |           [✅][OutdoorMeterWebhook]           |                      -                       |      ✅       |
| Weather Station [[JP][WeatherStationProductJP]]                                                                                    |           [✅][WeatherStationList]           |          [✅][WeatherStationStatus]          |          [✅][WeatherStationWebhook]          |          [✅][WeatherStationCommand]          |              |
| [Motion Sensor][MotionSensorProduct] [[JP][MotionSensorProductJP]]                                                                 |            [✅][MotionSensorList]            |           [✅][MotionSensorStatus]           |           [✅][MotionSensorWebhook]           |                      -                       |      ✅       |
| [Presence Sensor][PresenceSensorProduct] [[JP][PresenceSensorProductJP]]                                                           |           [✅][PresenceSensorList]           |          [✅][PresenceSensorStatus]          |          [✅][PresenceSensorWebhook]          |                      -                       |      ✅       |
| [Contact Sensor][ContactSensorProduct] [[JP][ContactSensorProductJP]]                                                              |           [✅][ContactSensorList]            |          [✅][ContactSensorStatus]           |          [✅][ContactSensorWebhook]           |                      -                       |      ✅       |
| [Water Leak Detector][WaterLeakDetectorProduct] [[JP][WaterLeakDetectorProductJP]]                                                 |         [✅][WaterLeakDetectorList]          |        [✅][WaterLeakDetectorStatus]         |        [✅][WaterLeakDetectorWebhook]         |                      -                       |              |
| **Security(Lock)**                                                                                                                 |                      -                      |                      -                      |                      -                       |                      -                       |      -       |
| [Smart Lock][SmartLockProduct] [[JP][SmartLockProductJP]]                                                                          |             [✅][SmartLockList]              |            [✅][SmartLockStatus]             |            [✅][SmartLockWebhook]             |            [✅][SmartLockCommand]             |      ✅       |
| [Smart Lock Pro][SmartLockProProduct] [[JP][SmartLockProProductJP]]                                                                |            [✅][SmartLockProList]            |           [✅][SmartLockProStatus]           |           [✅][SmartLockProWebhook]           |           [✅][SmartLockProCommand]           |              |
| [Smart Lock Lite][SmartLockLiteProduct] [[JP][SmartLockLiteProductJP]]                                                             |           [✅][SmartLockLiteList]            |          [✅][SmartLockLiteStatus]           |          [✅][SmartLockLiteWebhook]           |          [✅][SmartLockLiteCommand]           |              |
| [Smart Lock Ultra][SmartLockUltraProduct] [[JP][SmartLockUltraProductJP]]                                                          |           [✅][SmartLockUltraList]           |          [✅][SmartLockUltraStatus]          |          [✅][SmartLockUltraWebhook]          |          [✅][SmartLockUltraCommand]          |      ✅       |
| [Smart Lock Pro (Matter Enabled)][LockProMatterEnabledProduct]                                                                     |        [✅][LockProMatterEnabledList]        |       [✅][LockProMatterEnabledStatus]       |       [✅][LockProMatterEnabledWebhook]       |       [✅][LockProMatterEnabledCommand]       |              |
| [Lock Vision][LockVisionProduct]                                                                                                   |             [✅][LockVisionList]             |            [✅][LockVisionStatus]            |            [✅][LockVisionWebhook]            |            [✅][LockVisionCommand]            |              |
| [Lock Vision Pro][LockVisionProProduct]                                                                                            |           [✅][LockVisionProList]            |          [✅][LockVisionProStatus]           |          [✅][LockVisionProWebhook]           |          [✅][LockVisionProCommand]           |              |
| Keypad [JP][KeypadProductJP]                                                                                                       |               [✅][KeypadList]               |              [✅][KeypadStatus]              |              [✅][KeypadWebhook]              |              [✅][KeypadCommand]              |              |
| [Keypad Touch][KeypadTouchProduct] [[JP][KeypadTouchProductJP]]                                                                    |            [✅][KeypadTouchList]             |           [✅][KeypadTouchStatus]            |           [✅][KeypadTouchWebhook]            |           [✅][KeypadTouchCommand]            |      ✅       |
| [Keypad Vision][KeypadVisionProduct] [[JP][KeypadVisionProductJP]]                                                                 |            [✅][KeypadVisionList]            |           [✅][KeypadVisionStatus]           |           [✅][KeypadVisionWebhook]           |           [✅][KeypadVisionCommand]           |              |
| [Keypad Vision Pro][KeypadVisionProProduct] [[JP][KeypadVisionProProductJP]]                                                       |          [✅][KeypadVisionProList]           |         [✅][KeypadVisionProStatus]          |         [✅][KeypadVisionProWebhook]          |         [✅][KeypadVisionProCommand]          |              |
| **Security(Camera)**                                                                                                               |                      -                      |                      -                      |                      -                       |                      -                       |      -       |
| [Outdoor Spotlight Cam 1080P][OutdoorSpotlightCam1080PProduct] [[JP][OutdoorSpotlightCam1080PProductJP]]                           |                      -                      |                                             |                                              |                                              |              |
| [Outdoor Spotlight Cam 2K(3MP)][OutdoorSpotlightCam2K3MPProduct] [[JP][OutdoorSpotlightCam2K3MPProductJP]]                         |                      -                      |                      -                      |                      -                       |                      -                       |      -       |
| [Pan/Tilt Cam][PanTiltCamProduct] [[JP][PanTiltCamProductJP]]                                                                      |             [✅][PanTiltCamList]             |                      -                      |            [✅][PanTiltCamWebhook]            |                      -                       |              |
| [Pan/Tilt Cam 2K(3MP)][PanTiltCam2K3MPProduct] [[JP][PanTiltCam2K3MPProductJP]]                                                    |          [✅][PanTiltCam2K3MPList]           |                                             |                                              |                                              |              |
| [Pan/Tilt Cam Plus 2K(3MP)][PanTiltCamPlus3MPProduct] [[JP][PanTiltCamPlus3MPProductJP]]                                           |         [✅][PanTiltCamPlus3MPList]          |                     ✅📸                     |                      ✅📸                      |                      -                       |              |
| [Pan/Tilt Cam Plus 3K(5MP)][PanTiltCamPlus5MPProduct] [[JP][PanTiltCamPlus5MPProductJP]]                                           |         [✅][PanTiltCamPlus5MPList]          |                     ✅📸                     |                      ✅📸                      |                      -                       |      ✅       |
| [Outdoor Pan/Tilt Cam 3K][OutdoorPanTiltCam3KProduct] [[JP][OutdoorPanTiltCam3KProductJP]]                                         |                      -                      |                     ✅📸                     |                      ✅📸                      |                      -                       |      ✅       |
| [Indoor Cam][IndoorCamProduct] [[JP][IndoorCamProductJP]]                                                                          |             [✅][IndoorCamList]              |                      -                      |            [✅][IndoorCamWebhook]             |                      -                       |      ✅       |
| [Video Doorbell][VideoDoorbellProduct] [[JP][VideoDoorbellProductJP]]                                                              |           [✅][VideoDoorbellList]            |          [✅][VideoDoorbellStatus]           |          [✅][VideoDoorbellWebhook]📸          |          [✅][VideoDoorbellCommand]           |      ✅       |
| **Power & Switch**                                                                                                                 |                      -                      |                      -                      |                      -                       |                      -                       |      -       |
| Plug                                                                                                                               |                [✅][PlugList]                |               [✅][PlugStatus]               |                      -                       |               [✅][PlugCommand]               |              |
| [Plug Mini (US)][PlugMiniUSProduct]                                                                                                |             [✅][PlugMiniUSList]             |            [✅][PlugMiniUSStatus]            |            [✅][PlugMiniUSWebhook]            |            [✅][PlugMiniUSCommand]            |              |
| Plug Mini (JP) [[JP][PlugMiniJPProductJP]]                                                                                         |             [✅][PlugMiniJPList]             |            [✅][PlugMiniJPStatus]            |            [✅][PlugMiniJPWebhook]            |            [✅][PlugMiniJPCommand]            |      ✅       |
| [Plug Mini (EU)][PlugMiniEUProduct]                                                                                                |             [✅][PlugMiniEUList]             |            [✅][PlugMiniEUStatus]            |            [✅][PlugMiniEUWebhook]            |            [✅][PlugMiniEUCommand]            |              |
| [Remote][RemoteProduct] [[JP][RemoteProductJP]]                                                                                    |               [✅][RemoteList]               |                      -                      |                      -                       |                      -                       |      -       |
| **Lighting**                                                                                                                       |                      -                      |                      -                      |                      -                       |                      -                       |      -       |
| Ceiling Light [[JP][CeilingLightProductJP]]                                                                                        |            [✅][CeilingLightList]            |           [✅][CeilingLightStatus]           |           [✅][CeilingLightWebhook]           |           [✅][CeilingLightCommand]           |      ✅       |
| Ceiling Light Pro [[JP][CeilingLightProProductJP]]                                                                                 |          [✅][CeilingLightProList]           |         [✅][CeilingLightProStatus]          |         [✅][CeilingLightProWebhook]          |         [✅][CeilingLightProCommand]          |              |
| [Color Bulb][ColorBulbProduct] [[JP][ColorBulbProductJP]]                                                                          |             [✅][ColorBulbList]              |            [✅][ColorBulbStatus]             |            [✅][ColorBulbWebhook]             |            [✅][ColorBulbCommand]             |      ✅       |
| [Strip Light][StripLightProduct] [[JP][StripLightProductJP]]<br>Strip Light2 [[JP][StripLight2ProductJP]]                          |             [✅][StripLightList]             |            [✅][StripLightStatus]            |            [✅][StripLightWebhook]            |            [✅][StripLightCommand]            |      ✅       |
| [RGBWW Strip Light 3][StripLight3Product] [[JP][StripLight3ProductJP]]                                                             |            [✅][StripLight3List]             |           [✅][StripLight3Status]            |           [✅][StripLight3Webhook]            |           [✅][StripLight3Command]            |      ✅       |
| [RGBWW Floor Lamp][FloorLampProduct] [[JP][FloorLampProductJP]]                                                                    |             [✅][FloorLampList]              |            [✅][FloorLampStatus]             |            [✅][FloorLampWebhook]             |            [✅][FloorLampCommand]             |              |
| [RGBICWW Strip Light][RGBICWWStripLightProduct] [[JP][RGBICWWStripLightProductJP]]                                                 |         [✅][RGBICWWStripLightList]          |        [✅][RGBICWWStripLightStatus]         |        [✅][RGBICWWStripLightWebhook]         |        [✅][RGBICWWStripLightCommand]         |              |
| [RGBICWW Floor Lamp][RGBICWWFlooLampProduct] [[JP][RGBICWWFlooLampProductJP]]                                                      |          [✅][RGBICWWFlooLampList]           |         [✅][RGBICWWFlooLampStatus]          |         [✅][RGBICWWFlooLampWebhook]          |         [✅][RGBICWWFlooLampCommand]          |              |
| [RGBICWW Ceiling Light][RGBICWWCeilingLightProduct]                                                                                |       [✅][RGBICWWCeilingLightList]        |      [✅][RGBICWWCeilingLightStatus]       |      [✅][RGBICWWCeilingLightWebhook]      |      [✅][RGBICWWCeilingLightCommand]      |              |
| Permanent Outdoor Lights [[JP][PermanentOutdoorLightsProductJP]]                                                                   |     [✅][PermanentOutdoorLightsList]      |    [✅][PermanentOutdoorLightsStatus]     |    [✅][PermanentOutdoorLightsWebhook]    |    [✅][PermanentOutdoorLightsCommand]    |              |
| [RGBIC Neon Wire Rope Light][RGBICNeonWireRopeLightProduct] [[JP][RGBICNeonWireRopeLightProductJP]]                                |       [✅][RGBICNeonWireRopeLightList]       |      [✅][RGBICNeonWireRopeLightStatus]      |      [✅][RGBICNeonWireRopeLightWebhook]      |      [✅][RGBICNeonWireRopeLightCommand]      |      ✅       |
| [RGBIC Neon Rope Light][RGBICNeonRopeLightProduct] [[JP][RGBICNeonRopeLightProductJP]]                                             |         [✅][RGBICNeonRopeLightList]         |        [✅][RGBICNeonRopeLightStatus]        |        [✅][RGBICNeonRopeLightWebhook]        |        [✅][RGBICNeonRopeLightCommand]        |              |
| [Candle Warmer Lamp][CandleWarmerLampProduct] [[JP][CandleWarmerLampProductJP]]                                                    |          [✅][CandleWarmerLampList]          |         [✅][CandleWarmerLampStatus]         |         [✅][CandleWarmerLampWebhook]         |         [✅][CandleWarmerLampCommand]         |              |

* The 📸 icon indicates that snapshots included in the Webhook payload are detected as [MQTT Image](https://www.home-assistant.io/integrations/image.mqtt/).
* The Feature APIs of Kata Friends (diary) and AI MindClip (recordings, summaries, todos, assistant) are out of scope for this MQTT bridge.
* If you see errors such as **`unknown webhook payload`** or **`unknown status payload`** in the add-on logs, the API response or webhook body may have introduced new fields. Please share the relevant logs and API response or webhook body in a GitHub Issue.

[GetDeviceList]: https://github.com/OpenWonderLabs/SwitchBotAPI#get-device-list
[StatusAPI]: https://github.com/OpenWonderLabs/SwitchBotAPI#get-device-status
[Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#webhook
[CommandAPI]: https://github.com/OpenWonderLabs/SwitchBotAPI#send-device-control-commands
[HubList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/hubs/hubhub-plushub-minihub-2hub-3.md#device-list-information
[HubPlusList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/hubs/hubhub-plushub-minihub-2hub-3.md#device-list-information
[HubMiniProduct]: https://www.switch-bot.com/products/switchbot-hub-mini
[HubMiniProductJP]: https://www.switchbot.jp/products/switchbot-hub-mini
[HubMiniList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/hubs/hubhub-plushub-minihub-2hub-3.md#device-list-information
[Hub2Product]: https://www.switch-bot.com/pages/switchbot-hub-2
[Hub2ProductJP]: https://www.switchbot.jp/products/switchbot-hub2
[Hub2List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/hubs/hubhub-plushub-minihub-2hub-3.md#device-list-information
[Hub2Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/hubs/hub-2.md#device-status
[Hub2Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/hubs/hub-2.md#webhook-events
[Hub3Product]: https://www.switch-bot.com/pages/switchbot-hub-3
[Hub3ProductJP]: https://www.switchbot.jp/products/switchbot-hub3
[Hub3List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/hubs/hubhub-plushub-minihub-2hub-3.md#device-list-information
[Hub3Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/hubs/hub-3.md#device-status
[Hub3Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/hubs/hub-3.md#webhook-events
[AIHubProduct]: https://www.switch-bot.com/products/switchbot-ai-hub
[AIHubProductJP]: https://www.switchbot.jp/products/switchbot-aihub
[AIHubList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/hubs/ai-hub.md#device-list-information
[AIHubStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/hubs/ai-hub.md#device-status
[BotProduct]: https://www.switch-bot.com/products/switchbot-bot
[BotProductJP]: https://www.switchbot.jp/products/switchbot-bot
[BotList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/bot.md#device-list-information
[BotStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/bot.md#device-status
[BotWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/bot.md#webhook-events
[BotCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/bot.md#control-commands
[CurtainProduct]: https://www.switch-bot.com/products/switchbot-curtain
[CurtainList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/curtain.md#device-list-information
[CurtainStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/curtain.md#device-status
[CurtainWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/curtain.md#webhook-events
[CurtainCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/curtain.md#control-commands
[Curtain3Product]: https://www.switch-bot.com/products/switchbot-curtain-3
[Curtain3ProductJP]: https://www.switchbot.jp/products/switchbot-curtain3
[Curtain3List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/curtain-3.md#device-list-information
[Curtain3Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/curtain-3.md#device-status
[Curtain3Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/curtain-3.md#webhook-events
[Curtain3Command]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/curtain-3.md#control-commands
[BlindTiltProduct]: https://www.switch-bot.com/products/switchbot-blind-tilt
[BlindTiltProductJP]: https://www.switchbot.jp/products/switchbot-blind-tilt
[BlindTiltList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/blind-tilt.md#device-list-information
[BlindTiltStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/blind-tilt.md#device-status
[BlindTiltCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/blind-tilt.md#control-commands
[RollerShadeProduct]: https://www.switch-bot.com/products/switchbot-roller-shade
[RollerShadeList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/roller-shade.md#device-list-information
[RollerShadeStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/roller-shade.md#device-status
[RollerShadeWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/roller-shade.md#webhook-events
[RollerShadeCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/curtains-blinds/roller-shade.md#control-commands
[UniversalRemoteProduct]: https://www.switch-bot.com/products/switchbot-universal-remote
[UniversalRemoteProductJP]: https://www.switchbot.jp/products/switchbot-universal-remote
[WalletFinderCardProduct]: https://www.switch-bot.com/products/switchbot-wallet-finder-card
[WalletFinderCardProductJP]: https://www.switchbot.jp/products/switchbot-wallet-finder-card
[RelaySwitch1PMProduct]: https://www.switch-bot.com/products/switchbot-relay-switch-1pm
[RelaySwitch1PMList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/relay-switch-1pm.md#device-list-information
[RelaySwitch1PMStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/relay-switch-1pm.md#device-status
[RelaySwitch1PMWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/relay-switch-1pm.md#webhook-events
[RelaySwitch1PMCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/relay-switch-1pm.md#control-commands
[RelaySwitch1Product]: https://www.switch-bot.com/products/switchbot-relay-switch-1
[RelaySwitch1ProductJP]: https://www.switchbot.jp/products/switchbot-relay-switch-1
[RelaySwitch1List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/relay-switch-1.md#device-list-information
[RelaySwitch1Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/relay-switch-1.md#device-status
[RelaySwitch1Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/relay-switch-1.md#webhook-events
[RelaySwitch1Command]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/relay-switch-1.md#control-commands
[RelaySwitch2PMProduct]: https://www.switch-bot.com/products/switchbot-relay-switch-2pm
[RelaySwitch2PMList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/relay-switch-2pm.md#device-list-information
[RelaySwitch2PMStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/relay-switch-2pm.md#device-status
[RelaySwitch2PMWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/relay-switch-2pm.md#webhook-events
[RelaySwitch2PMCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/relay-switch-2pm.md#control-commands
[GarageDoorOpenerProduct]: https://www.switch-bot.com/products/switchbot-garage-door-opener
[GarageDoorOpenerList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/garage-door-opener.md#device-list-information
[GarageDoorOpenerStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/garage-door-opener.md#device-status
[GarageDoorOpenerWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/garage-door-opener.md#webhook-events
[GarageDoorOpenerCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/garage-door-opener.md#control-commands
[SmartRadiatorThermostatProduct]: https://www.switch-bot.com/products/switchbot-smart-radiator-thermostat
[SmartRadiatorThermostatList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/smart-radiator-thermostat.md#device-list-information
[SmartRadiatorThermostatStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/smart-radiator-thermostat.md#device-status
[SmartRadiatorThermostatWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/smart-radiator-thermostat.md#webhook-events
[SmartRadiatorThermostatCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/smart-radiator-thermostat.md#control-commands
[HumidifierProductJP]: https://www.switchbot.jp/products/switchbot-smart-humidifier?variant=40981225799855
[PresenceSensorProduct]: https://www.switch-bot.com/products/switchbot-presence-sensor
[PresenceSensorProductJP]: https://www.switchbot.jp/products/switchbot-presence-sensor
[PresenceSensorList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/presence-sensor.md#device-list-information
[PresenceSensorStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/presence-sensor.md#device-status
[PresenceSensorWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/presence-sensor.md#webhook-events
[HumidifierList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/humidifier.md#device-list-information
[HumidifierStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/humidifier.md#device-status
[HumidifierCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/humidifier.md#control-commands
[EvaporativeHumidifierProductJP]: https://www.switchbot.jp/products/switchbot-evaporative-humidifier
[EvaporativeHumidifierList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/evaporative-humidifier.md#device-list-information
[EvaporativeHumidifierStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/evaporative-humidifier.md#device-status
[EvaporativeHumidifierWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/evaporative-humidifier.md#webhook-events
[EvaporativeHumidifierCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/evaporative-humidifier.md#control-commands
[EvaporativeHumidifierAutoRefillProduct]: https://us.switch-bot.com/products/switchbot-evaporative-humidifier-auto-refill
[EvaporativeHumidifierAutoRefillProductJP]: https://www.switchbot.jp/products/switchbot-evaporative-humidifier-plus
[EvaporativeHumidifierAutoRefillList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/evaporative-humidifier-auto-refill.md#device-list-information
[EvaporativeHumidifierAutoRefillStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/evaporative-humidifier-auto-refill.md#device-status
[EvaporativeHumidifierAutoRefillWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/evaporative-humidifier-auto-refill.md#webhook-events
[EvaporativeHumidifierAutoRefillCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/evaporative-humidifier-auto-refill.md#control-commands
[BatteryCirculatorFanProduct]: https://www.switch-bot.com/products/switchbot-battery-circulator-fan?variant=46175199756455
[BatteryCirculatorFanProductJP]: https://www.switchbot.jp/products/switchbot-smart-circulator-fan?variant=44020075167919
[BatteryCirculatorFanList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/battery-circulator-fan.md#device-list-information
[BatteryCirculatorFanStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/battery-circulator-fan.md#device-status
[BatteryCirculatorFanWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/battery-circulator-fan.md#webhook-events
[BatteryCirculatorFanCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/battery-circulator-fan.md#control-commands
[BatteryCirculatorFan2ProProductJP]: https://www.switchbot.jp/products/switchbot-smart-circulator-fan-2-pro
[BatteryCirculatorFan2ProList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/battery-circulator-fan-2-pro.md#device-list-information
[BatteryCirculatorFan2ProStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/battery-circulator-fan-2-pro.md#device-status
[BatteryCirculatorFan2ProWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/battery-circulator-fan-2-pro.md#webhook-events
[BatteryCirculatorFan2ProCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/battery-circulator-fan-2-pro.md#control-commands
[CirculatorFanProduct]: https://www.switch-bot.com/products/switchbot-battery-circulator-fan?variant=46175199789223
[CirculatorFanProductJP]: https://www.switchbot.jp/products/switchbot-smart-circulator-fan?variant=44221010182319
[CirculatorFanList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/circulator-fan.md#device-list-information
[CirculatorFanStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/circulator-fan.md#device-status
[CirculatorFanWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/circulator-fan.md#webhook-events
[CirculatorFanCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/circulator-fan.md#control-commands
[!StandingCirculatorFanProduct]: https://www.switch-bot.com/products/switchbot-standing-circulator-fan
[!StandingCirculatorFanProductJP]: https://www.switchbot.jp/products/switchbot-standing-circulator-fan
[StandingCirculatorFanList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/standing-circulator-fan.md#device-list-information
[StandingCirculatorFanStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/standing-circulator-fan.md#device-status
[StandingCirculatorFanWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/standing-circulator-fan.md#webhook-events
[StandingCirculatorFanCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/standing-circulator-fan.md#control-commands
[AirPurifierPM25Product]: https://www.switch-bot.com/products/switchbot-air-purifier
[AirPurifierPM25List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-pm25.md#device-list-information
[AirPurifierPM25Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-pm25.md#device-status
[AirPurifierPM25Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-pm25.md#webhook-events
[AirPurifierPM25Command]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-pm25.md#control-commands
[AirPurifierTablePM25Product]: https://www.switch-bot.com/products/switchbot-air-purifier-table
[AirPurifierTablePM25List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-table-pm25.md#device-list-information
[AirPurifierTablePM25Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-table-pm25.md#device-status
[AirPurifierTablePM25Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-table-pm25.md#webhook-events
[AirPurifierTablePM25Command]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-table-pm25.md#control-commands
[AirPurifierVOCProductJP]: https://www.switchbot.jp/products/switchbot-air-purifier
[AirPurifierVOCList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-voc.md#device-list-information
[AirPurifierVOCStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-voc.md#device-status
[AirPurifierVOCWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-voc.md#webhook-events
[AirPurifierVOCCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-voc.md#control-commands
[AirPurifierTableVOCProductJP]: https://www.switchbot.jp/products/switchbot-air-purifier-table
[AirPurifierTableVOCList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-table-voc.md#device-list-information
[AirPurifierTableVOCStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-table-voc.md#device-status
[AirPurifierTableVOCWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-table-voc.md#webhook-events
[AirPurifierTableVOCCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/air-purifier-table-voc.md#control-commands
[AIArtFrameProduct]: https://www.switch-bot.com/products/switchbot-ai-art-frame
[AIArtFrameProductJP]: https://www.switchbot.jp/products/switchbot-ai-art-frame
[AIArtFrameList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/ai-art-frame.md#device-list-information
[AIArtFrameStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/ai-art-frame.md#device-status
[AIArtFrameWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/ai-art-frame.md#webhook-events
[AIArtFrameCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/ai-art-frame.md#control-commands
[RobotVacuumCleanerS1ProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner?variant=41850919420079
[RobotVacuumCleanerS1List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/robot-vacuum-cleaner-s1.md#device-list-information
[RobotVacuumCleanerS1Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/robot-vacuum-cleaner-s1.md#device-status
[RobotVacuumCleanerS1Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/robot-vacuum-cleaner-s1.md#webhook-events
[RobotVacuumCleanerS1Command]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/robot-vacuum-cleaner-s1.md#control-commands
[RobotVacuumCleanerS1PlusProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner?variant=44254800347311
[RobotVacuumCleanerS1PlusList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/robot-vacuum-cleaner-s1-plus.md#device-list-information
[RobotVacuumCleanerS1PlusStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/robot-vacuum-cleaner-s1-plus.md#device-status
[RobotVacuumCleanerS1PlusWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/robot-vacuum-cleaner-s1-plus.md#webhook-events
[RobotVacuumCleanerS1PlusCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/robot-vacuum-cleaner-s1-plus.md#control-commands
[MiniRobotVacuumK10+Product]: https://www.switch-bot.com/products/switchbot-mini-robot-vacuum-k10
[MiniRobotVacuumK10+ProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner-k10
[MiniRobotVacuumK10+List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/mini-robot-vacuum-k10.md#device-list-information
[MiniRobotVacuumK10+Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/mini-robot-vacuum-k10.md#device-status
[MiniRobotVacuumK10+Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/mini-robot-vacuum-k10.md#webhook-events
[MiniRobotVacuumK10+Command]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/mini-robot-vacuum-k10.md#control-commands
[MiniRobotVacuumK10+ProProduct]: https://www.switch-bot.com/products/switchbot-mini-robot-vacuum-k10-pro
[MiniRobotVacuumK10+ProProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner-k10-pro
[MiniRobotVacuumK10+ProList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/mini-robot-vacuum-k10-pro.md#device-list-information
[MiniRobotVacuumK10+ProStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/mini-robot-vacuum-k10-pro.md#device-status
[MiniRobotVacuumK10+ProWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/mini-robot-vacuum-k10-pro.md#webhook-events
[MiniRobotVacuumK10+ProCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/mini-robot-vacuum-k10-pro.md#control-commands
[MultitaskingHouseholdRobotK20ProProduct]: https://www.switch-bot.com/products/switchbot-mini-robot-vacuum-k20-pro
[MultitaskingHouseholdRobotK20ProProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner-k20-pro
[MultitaskingHouseholdRobotK20ProList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/k20-pro.md#device-list-information
[MultitaskingHouseholdRobotK20ProStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/k20-pro.md#device-status
[MultitaskingHouseholdRobotK20ProWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/k20-pro.md#webhook-events
[MultitaskingHouseholdRobotK20ProCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/k20-pro.md#control-commands
[FloorCleaningRobotS10Product]: https://www.switch-bot.com/products/switchbot-floor-cleaning-robot-s10
[FloorCleaningRobotS10ProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner-s10
[FloorCleaningRobotS10List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/floor-cleaning-robot-s10.md#device-list-information
[FloorCleaningRobotS10Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/floor-cleaning-robot-s10.md#device-status
[FloorCleaningRobotS10Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/floor-cleaning-robot-s10.md#webhook-events
[FloorCleaningRobotS10Command]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/floor-cleaning-robot-s10.md#control-commands
[FloorCleaningRobotS20Product]: https://www.switch-bot.com/products/switchbot-floor-cleaning-robot-s20
[FloorCleaningRobotS20ProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner-s20
[FloorCleaningRobotS20List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/floor-cleaning-robot-s20.md#device-list-information
[FloorCleaningRobotS20Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/floor-cleaning-robot-s20.md#device-status
[FloorCleaningRobotS20Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/floor-cleaning-robot-s20.md#webhook-events
[FloorCleaningRobotS20Command]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/floor-cleaning-robot-s20.md#control-commands
[RobotVacuumK10+ProComboProduct]: https://www.switch-bot.com/products/switchbot-k10-pro-combo
[RobotVacuumK10+ProComboProductJP]: https://www.switchbot.jp/products/robot-vacuum-cleaner-k10-pro-combo
[RobotVacuumK10+ProComboList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/k10-pro-combo.md#device-list-information
[RobotVacuumK10+ProComboStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/k10-pro-combo.md#device-status
[RobotVacuumK10+ProComboWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/k10-pro-combo.md#webhook-events
[RobotVacuumK10+ProComboCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/k10-pro-combo.md#control-commands
[RobotVacuumK11+Product]: https://www.switch-bot.com/products/switchbot-robot-vacuum-k11
[RobotVacuumK11+ProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner-k11
[RobotVacuumK11+List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/robot-vacuum-k11.md#device-list-information
[RobotVacuumK11+Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/robot-vacuum-k11.md#device-status
[RobotVacuumK11+Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/robot-vacuum-k11.md#webhook-events
[RobotVacuumK11+Command]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/robot-vacuum/robot-vacuum-k11.md#control-commands
[MeterProduct]: https://www.switch-bot.com/products/switchbot-meter
[MeterProductJP]: https://www.switchbot.jp/products/switchbot-meter
[MeterList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/meter.md#device-list-information
[MeterStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/meter.md#device-status
[MeterWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/meter.md#webhook-events
[MeterPlusProduct]: https://www.switch-bot.com/products/switchbot-meter-plus
[MeterPlusProductJP]: https://www.switchbot.jp/products/switchbot-meter-plus
[MeterPlusList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/meter-plus.md#device-list-information
[MeterPlusStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/meter-plus.md#device-status
[MeterPlusWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/meter-plus.md#webhook-events
[MeterProProduct]: https://www.switch-bot.com/products/switchbot-meter-pro
[MeterProProductJP]: https://www.switchbot.jp/products/switchbot-meter-pro
[MeterProList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/meter-pro.md#device-list-information
[MeterProStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/meter-pro.md#device-status
[MeterProWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/meter-pro.md#webhook-events
[MeterProCO2Product]: https://www.switch-bot.com/products/switchbot-meter-pro-co2-monitor
[MeterProCO2ProductJP]: https://www.switchbot.jp/products/switchbot-co2-meter
[MeterProCO2List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/meter-pro-co2.md#device-list-information
[MeterProCO2Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/meter-pro-co2.md#device-status
[MeterProCO2Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/meter-pro-co2.md#webhook-events
[OutdoorMeterProduct]: https://www.switch-bot.com/products/switchbot-indoor-outdoor-thermo-hygrometer
[OutdoorMeterProductJP]: https://www.switchbot.jp/products/switchbot-indoor-outdoor-meter
[OutdoorMeterList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/outdoor-meter.md#device-list-information
[OutdoorMeterStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/outdoor-meter.md#device-status
[OutdoorMeterWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/outdoor-meter.md#webhook-events
[WeatherStationProductJP]: https://www.switchbot.jp/products/switchbot-weather-station
[WeatherStationList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/weather-station.md#device-list-information
[WeatherStationStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/weather-station.md#device-status
[WeatherStationWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/weather-station.md#webhook-events
[WeatherStationCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/weather-station.md#control-commands
[MotionSensorProduct]: https://www.switch-bot.com/products/motion-sensor
[MotionSensorProductJP]: https://www.switchbot.jp/products/switchbot-motion-sensor
[MotionSensorList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/motion-sensor.md#device-list-information
[MotionSensorStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/motion-sensor.md#device-status
[MotionSensorWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/motion-sensor.md#webhook-events
[ClimatePanelProduct]: https://www.switch-bot.com/products/switchbot-home-climate-panel
[ClimatePanelProductJP]: https://www.switchbot.jp/products/switchbot-climate-panel
[ClimatePanelList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/home-climate-panel.md#device-list-information
[ClimatePanelStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/home-climate-panel.md#device-status
[ClimatePanelWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/climate-control/home-climate-panel.md#webhook-events
[KataFriendsProduct]: https://www.switch-bot.com/products/katafriends
[KataFriendsProductJP]: https://www.switchbot.jp/products/katafriends
[KataFriendsList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/kata-friends.md#device-list-information
[KataFriendsStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/kata-friends.md#device-status
[KataFriendsWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/kata-friends.md#webhook-events
[KataFriendsCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/kata-friends.md#control-commands
[AIMindClipList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/ai-mindclip.md#device-list-information
[AIMindClipStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/ai-mindclip.md#device-status
[AIMindClipWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/ai-mindclip.md#webhook-events
[ContactSensorProduct]: https://www.switch-bot.com/products/contact-sensor
[ContactSensorProductJP]: https://www.switchbot.jp/products/switchbot-contact-sensor
[ContactSensorList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/contact-sensor.md#device-list-information
[ContactSensorStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/contact-sensor.md#device-status
[ContactSensorWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/contact-sensor.md#webhook-events
[WaterLeakDetectorProduct]: https://www.switch-bot.com/products/switchbot-water-leak-detector
[WaterLeakDetectorProductJP]: https://www.switchbot.jp/products/switchbot-water-leak-detector
[WaterLeakDetectorList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/water-leak-detector.md#device-list-information
[WaterLeakDetectorStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/water-leak-detector.md#device-status
[WaterLeakDetectorWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/sensors/water-leak-detector.md#webhook-events
[SmartLockProduct]: https://www.switch-bot.com/products/switchbot-lock
[SmartLockProductJP]: https://www.switchbot.jp/products/switchbot-lock
[SmartLockList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock.md#device-list-information
[SmartLockStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock.md#device-status
[SmartLockWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock.md#webhook-events
[SmartLockCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock.md#control-commands
[SmartLockProProduct]: https://www.switch-bot.com/products/switchbot-lock-pro
[SmartLockProProductJP]: https://www.switchbot.jp/products/switchbot-lock-pro
[SmartLockProList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-pro.md#device-list-information
[SmartLockProStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-pro.md#device-status
[SmartLockProWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-pro.md#webhook-events
[SmartLockProCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-pro.md#control-commands
[SmartLockLiteProduct]: https://www.switch-bot.com/products/switchbot-lock-lite
[SmartLockLiteProductJP]: https://www.switchbot.jp/products/switchbot-lock-lite
[SmartLockLiteList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-lite.md#device-list-information
[SmartLockLiteStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-lite.md#device-status
[SmartLockLiteWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-lite.md#webhook-events
[SmartLockLiteCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-lite.md#control-commands
[SmartLockUltraProduct]: https://www.switch-bot.com/products/switchbot-lock-ultra
[SmartLockUltraProductJP]: https://www.switchbot.jp/products/switchbot-lock-ultra
[SmartLockUltraList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-ultra.md#device-list-information
[SmartLockUltraStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-ultra.md#device-status
[SmartLockUltraWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-ultra.md#webhook-events
[SmartLockUltraCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-ultra.md#control-commands
[LockProMatterEnabledProduct]: https://www.switch-bot.com/products/switchbot-lock-pro-matter-enabled
[LockProMatterEnabledList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-pro-matter-enabled.md#device-list-information
[LockProMatterEnabledStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-pro-matter-enabled.md#device-status
[LockProMatterEnabledWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-pro-matter-enabled.md#webhook-events
[LockProMatterEnabledCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-pro-matter-enabled.md#control-commands
[LockVisionProduct]: https://us.switch-bot.com/products/switchbot-lock-vision
[LockVisionList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-vision.md#device-list-information
[LockVisionStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-vision.md#device-status
[LockVisionWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-vision.md#webhook-events
[LockVisionCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-vision.md#control-commands
[LockVisionProProduct]: https://us.switch-bot.com/products/switchbot-lock-vision-pro
[LockVisionProList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-vision-pro.md#device-list-information
[LockVisionProStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-vision-pro.md#device-status
[LockVisionProWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-vision-pro.md#webhook-events
[LockVisionProCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/lock-vision-pro.md#control-commands
[KeypadProductJP]: https://www.switchbot.jp/products/switchbot-keypad
[KeypadList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad.md#device-list-information
[KeypadStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad.md#device-status
[KeypadWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad.md#webhook-events
[KeypadCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad.md#control-commands
[KeypadTouchProduct]: https://switch-bot.com/pages/switchbot-keypad
[KeypadTouchProductJP]: https://www.switchbot.jp/products/switchbot-keypad-touch
[KeypadTouchList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad-touch.md#device-list-information
[KeypadTouchStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad-touch.md#device-status
[KeypadTouchWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad-touch.md#webhook-events
[KeypadTouchCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad-touch.md#control-commands
[KeypadVisionProduct]: https://www.switch-bot.com/products/switchbot-keypad-vision
[KeypadVisionProductJP]: https://www.switchbot.jp/products/switchbot-keypad-vision
[KeypadVisionList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad-vision.md#device-list-information
[KeypadVisionStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad-vision.md#device-status
[KeypadVisionWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad-vision.md#webhook-events
[KeypadVisionCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad-vision.md#control-commands
[KeypadVisionProProduct]: https://www.switch-bot.com/products/switchbot-keypad-vision-pro
[KeypadVisionProProductJP]: https://www.switchbot.jp/products/switchbot-keypad-vision-pro
[KeypadVisionProList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad-vision-pro.md#device-list-information
[KeypadVisionProStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad-vision-pro.md#device-status
[KeypadVisionProWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad-vision-pro.md#webhook-events
[KeypadVisionProCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/locks-security/keypad-vision-pro.md#control-commands
[OutdoorSpotlightCam1080PProduct]: https://www.switch-bot.com/products/switchbot-outdoor-spotlight-cam?variant=43002833338535
[OutdoorSpotlightCam1080PProductJP]: https://www.switchbot.jp/products/switchbot-outdoor-spotlight-cam
[OutdoorSpotlightCam2K3MPProduct]: https://www.switch-bot.com/products/switchbot-outdoor-spotlight-cam?variant=45882280738983
[OutdoorSpotlightCam2K3MPProductJP]: https://www.switchbot.jp/products/switchbot-outdoor-spotlight-cam-3mp
[PanTiltCamProduct]: https://switch-bot.com/pages/switchbot-pan-tilt-cam
[PanTiltCamProductJP]: https://www.switchbot.jp/products/switchbot-pan-tilt-cam
[PanTiltCamList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/cameras/pantilt-cam.md#device-list-information
[PanTiltCamWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/cameras/pantilt-cam.md#webhook-events
[PanTiltCam2K3MPProduct]: https://switch-bot.com/pages/switchbot-pan-tilt-cam-2k
[PanTiltCam2K3MPProductJP]: https://www.switchbot.jp/products/switchbot-pan-tilt-cam-3mp
[PanTiltCam2K3MPList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/cameras/pantilt-cam-2k.md#device-list-information
[PanTiltCamPlus3MPProduct]: https://us.switch-bot.com/pages/switchbot-pan-tilt-cam-plus-2k
[PanTiltCamPlus3MPProductJP]: https://www.switchbot.jp/products/switchbot-pan-tilt-cam-plus-3mp
[PanTiltCamPlus3MPList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/cameras/pantilt-cam-plus-2k.md#device-list-information
[PanTiltCamPlus5MPProduct]: https://us.switch-bot.com/pages/switchbot-pan-tilt-cam-plus-3k
[PanTiltCamPlus5MPProductJP]: https://www.switchbot.jp/products/switchbot-pan-tilt-cam-plus-5mp
[PanTiltCamPlus5MPList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/cameras/pantilt-cam-plus-3k.md#device-list-information
[OutdoorPanTiltCam3KProduct]: https://www.switch-bot.com/products/outdoor-ptc-3k5mp
[OutdoorPanTiltCam3KProductJP]: https://www.switchbot.jp/products/switchbot-outdoor-pan-tilt-cam-5mp
[IndoorCamProduct]: https://switch-bot.com/pages/switchbot-indoor-cam
[IndoorCamProductJP]: https://www.switchbot.jp/products/switchbot-indoor-cam
[IndoorCamList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/cameras/indoor-cam.md#device-list-information
[IndoorCamWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/cameras/indoor-cam.md#webhook-events
[VideoDoorbellProduct]: https://www.switch-bot.com/products/switchbot-video-doorbell
[VideoDoorbellProductJP]: https://www.switchbot.jp/products/switchbot-smart-video-doorbell
[VideoDoorbellList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/cameras/video-doorbell.md#device-list-information
[VideoDoorbellStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/cameras/video-doorbell.md#device-status
[VideoDoorbellWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/cameras/video-doorbell.md#webhook-events
[VideoDoorbellCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/cameras/video-doorbell.md#control-commands
[PlugList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug.md#device-list-information
[PlugStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug.md#device-status
[PlugCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug.md#control-commands
[PlugMiniUSProduct]: https://switch-bot.com/pages/switchbot-plug-mini
[PlugMiniUSList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug-mini-us.md#device-list-information
[PlugMiniUSStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug-mini-us.md#device-status
[PlugMiniUSWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug-mini-us.md#webhook-events
[PlugMiniUSCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug-mini-us.md#control-commands
[PlugMiniJPProductJP]: https://www.switchbot.jp/products/switchbot-plug-mini
[PlugMiniJPList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug-mini-jp.md#device-list-information
[PlugMiniJPStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug-mini-jp.md#device-status
[PlugMiniJPWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug-mini-jp.md#webhook-events
[PlugMiniJPCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug-mini-jp.md#control-commands
[PlugMiniEUProduct]: https://eu.switch-bot.com/products/switchbot-plug-mini
[PlugMiniEUList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug-mini-eu.md#device-list-information
[PlugMiniEUStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug-mini-eu.md#device-status
[PlugMiniEUWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug-mini-eu.md#webhook-events
[PlugMiniEUCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/plugs-switches/plug-mini-eu.md#control-commands
[RemoteProduct]: https://switch-bot.com/products/switchbot-remote
[RemoteProductJP]: https://www.switchbot.jp/products/switchbot-remote
[RemoteList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/remote.md#device-list-information
[CeilingLightProductJP]: https://www.switchbot.jp/products/switchbot-ceiling-light?variant=42442788438191
[CeilingLightList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/ceiling-light.md#device-list-information
[CeilingLightStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/ceiling-light.md#device-status
[CeilingLightWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/ceiling-light.md#webhook-events
[CeilingLightCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/ceiling-light.md#control-commands
[CeilingLightProProductJP]: https://www.switchbot.jp/products/switchbot-ceiling-light?variant=42442788503727
[CeilingLightProList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/ceiling-light-pro.md#device-list-information
[CeilingLightProStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/ceiling-light-pro.md#device-status
[CeilingLightProWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/ceiling-light-pro.md#webhook-events
[CeilingLightProCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/ceiling-light-pro.md#control-commands
[ColorBulbProduct]: https://www.switch-bot.com/products/switchbot-color-bulb
[ColorBulbProductJP]: https://www.switchbot.jp/products/switchbot-color-bulb
[ColorBulbList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/color-bulb.md#device-list-information
[ColorBulbStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/color-bulb.md#device-status
[ColorBulbWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/color-bulb.md#webhook-events
[ColorBulbCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/color-bulb.md#control-commands
[StripLightProduct]: https://www.switch-bot.com/products/switchbot-light-strip
[StripLightProductJP]: https://www.switchbot.jp/products/switchbot-strip-light
[StripLightList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/strip-light.md#device-list-information
[StripLightStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/strip-light.md#device-status
[StripLightWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/strip-light.md#webhook-events
[StripLightCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/strip-light.md#control-commands
[StripLight2ProductJP]: https://www.switchbot.jp/products/switchbot-strip-light2
[FloorLampProduct]: https://www.switch-bot.com/products/Floor-Lamp
[FloorLampProductJP]: https://www.switchbot.jp/products/switchbot-floor-lamp
[FloorLampList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/floor-lamp.md#device-list-information
[FloorLampStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/floor-lamp.md#device-status
[FloorLampWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/floor-lamp.md#webhook-events
[FloorLampCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/floor-lamp.md#control-commands
[StripLight3Product]: https://www.switch-bot.com/products/switchbot-led-strip-light-3
[StripLight3ProductJP]: https://www.switchbot.jp/products/switchbot-strip-light3
[StripLight3List]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/strip-light-3.md#device-list-information
[StripLight3Status]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/strip-light-3.md#device-status
[StripLight3Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/strip-light-3.md#webhook-events
[StripLight3Command]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/strip-light-3.md#control-commands
[RGBICWWStripLightProduct]: https://www.switch-bot.com/products/switchbot-rgbicww-strip-light
[RGBICWWStripLightProductJP]: https://www.switchbot.jp/products/switchbot-rgbicww-strip-light
[RGBICWWStripLightList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbicww-strip-light.md#device-list-information
[RGBICWWStripLightStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbicww-strip-light.md#device-status
[RGBICWWStripLightWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbicww-strip-light.md#webhook-events
[RGBICWWStripLightCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbicww-strip-light.md#control-commands
[RGBICWWFlooLampProduct]: https://www.switch-bot.com/products/switchbot-rgbicww-floor-lamp
[RGBICWWFlooLampProductJP]: https://www.switchbot.jp/products/switchbot-rgbicww-floor-lamp
[RGBICWWFlooLampList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbicww-floor-lamp.md#device-list-information
[RGBICWWFlooLampStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbicww-floor-lamp.md#device-status
[RGBICWWFlooLampWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbicww-floor-lamp.md#webhook-events
[RGBICWWFlooLampCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbicww-floor-lamp.md#control-commands
[RGBICWWCeilingLightProduct]: https://www.switch-bot.com/products/switchbot-rgbicww-ceiling-light
[RGBICWWCeilingLightList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbicww-ceiling-light.md#device-list-information
[RGBICWWCeilingLightStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbicww-ceiling-light.md#device-status
[RGBICWWCeilingLightWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbicww-ceiling-light.md#webhook-events
[RGBICWWCeilingLightCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbicww-ceiling-light.md#control-commands
[PermanentOutdoorLightsProductJP]: https://www.switchbot.jp/products/switchbot-permanent-outdoor-lights
[PermanentOutdoorLightsList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/permanent-outdoor-lights.md#device-list-information
[PermanentOutdoorLightsStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/permanent-outdoor-lights.md#device-status
[PermanentOutdoorLightsWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/permanent-outdoor-lights.md#webhook-events
[PermanentOutdoorLightsCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/permanent-outdoor-lights.md#control-commands
[RGBICNeonWireRopeLightProduct]: https://us.switch-bot.com/products/switchbot-rgbic-neon-wire-rope-light
[RGBICNeonWireRopeLightProductJP]: https://www.switchbot.jp/products/switchbot-rgbic-neon-wire-rope-light
[RGBICNeonWireRopeLightList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbic-neon-wire-rope-light.md#device-list-information
[RGBICNeonWireRopeLightStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbic-neon-wire-rope-light.md#device-status
[RGBICNeonWireRopeLightWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbic-neon-wire-rope-light.md#webhook-events
[RGBICNeonWireRopeLightCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbic-neon-wire-rope-light.md#control-commands
[RGBICNeonRopeLightProduct]: https://us.switch-bot.com/products/switchbot-rgbic-neon-rope-light
[RGBICNeonRopeLightProductJP]: https://www.switchbot.jp/products/switchbot-rgbic-neon-rope-light
[RGBICNeonRopeLightList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbic-neon-rope-light.md#device-list-information
[RGBICNeonRopeLightStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbic-neon-rope-light.md#device-status
[RGBICNeonRopeLightWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbic-neon-rope-light.md#webhook-events
[RGBICNeonRopeLightCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/rgbic-neon-rope-light.md#control-commands
[CandleWarmerLampProduct]: https://us.switch-bot.com/products/switchbot-candle-warmer-lamp
[CandleWarmerLampProductJP]: https://www.switchbot.jp/products/switchbot-candle-warmer-lamp
[CandleWarmerLampList]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/candle-warmer-lamp.md#device-list-information
[CandleWarmerLampStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/candle-warmer-lamp.md#device-status
[CandleWarmerLampWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/candle-warmer-lamp.md#webhook-events
[CandleWarmerLampCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/lighting/candle-warmer-lamp.md#control-commands

### Virtual Infrared Remote Devices

| Device               | [Command](https://github.com/OpenWonderLabs/SwitchBotAPI/blob/main/devices/others/virtual-infrared-remote-devices.md#control-commands) |
| -------------------- | :-------------------------------------------------------------------------------------------------------: |
| Air Conditioner      |                                                     ✅                                                     |
| TV                   |                                                     ✅                                                     |
| Light                |                                                     ✅                                                     |
| Fan                  |                                                     ✅                                                     |
| IPTV(Streamer)       |                                                     ✅                                                     |
| Set Top Box          |                                                     ✅                                                     |
| DVD Player           |                                                     ✅                                                     |
| Speaker              |                                                     ✅                                                     |
| Robot Vacuum Cleaner |                                                     ✅                                                     |
| Air Purifier         |                                                     ✅                                                     |
| Water Heater(Bath)   |                                                     ✅                                                     |
| Projector            |                                                     ✅                                                     |
| Camera               |                                                     ✅                                                     |
| Others               |                                                     ✅                                                     |

## Installation Method

For installation instructions, please refer to [here](https://github.com/hsakoh/switchbot-mqtt/wiki).
