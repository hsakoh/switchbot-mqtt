# SwitchBot MQTT Home Assistant add-on

This project is a Home Assistant add-on that allows you to control various SwitchBot products through the API.

The add-on can also receive Webhooks to obtain the device's status.
Via an MQTT broker, it will be detected as an MQTT integration in Home Assistant.

You can perform manual scene executions that were configured in the SwitchBot app, as well as control virtual infrared remote devices.

**Important: Please note that this add-on does not support operations on SwitchBot devices via Bluetooth.**

## Current Support Status

![aarch64-shield](https://img.shields.io/badge/aarch64-yes-green.svg)
![amd64-shield](https://img.shields.io/badge/amd64-yes-green.svg)
![armv7-shield](https://img.shields.io/badge/armv7-yes-green.svg)

### Physical Devices

We have implemented all devices according to the published API specifications, but testing has been conducted only on a subset.

* The "-" in the "Verification" column indicates that product was tested and found to have no specific functionality.
* To check which values can be referenced and what operations can be performed for each device, please refer to the links provided to each checkbox.
* Even for products not officially documented, the API may indicate the device type as another product. Additionally, devices with similar fields and commands may function if their device type is spoofed. For example, recognizing a K10+ as an S1 may enable operation. Configure the `EnforceDeviceTypes` option in your settings.

| Device                                                                                                      | [OpenAPI v1.1<br>Documented][GetDeviceList] |     [Status<br>API][StatusAPI]      |          [Webhook][Webhook]          |     [Command<br>API][CommandAPI]     | Verification |
|-------------------------------------------------------------------------------------------------------------|:-------------------------------------------:|:-----------------------------------:|:------------------------------------:|:------------------------------------:|:------------:|
| **Hub**                                                                                                     |                      -                      |                  -                  |                  -                   |                  -                   |      -       |
| Hub                                                                                                         |                [✅][HubList]                 |                  -                  |                  -                   |                  -                   |              |
| Hub Plus                                                                                                    |              [✅][HubPlusList]               |                  -                  |                  -                   |                  -                   |              |
| [Hub Mini][HubMiniProduct]  [[JP][HubMiniProductJP]]                                                        |              [✅][HubMiniList]               |                  -                  |                  -                   |                  -                   |      -       |
| [Hub 2][Hub2Product]  [[JP][Hub2ProductJP]]                                                                 |                [✅][Hub2List]                |           [✅][Hub2Status]           |           [✅][Hub2Webhook]           |                  -                   |      ✅       |
| **Home Automation**                                                                                         |                      -                      |                  -                  |                  -                   |                  -                   |      -       |
| [Bot][BotProduct]  [[JP][BotProductJP]]                                                                     |                [✅][BotList]                 |           [✅][BotStatus]            |           [✅][BotWebhook]            |           [✅][BotCommand]            |              |
| [Curtain][CurtainProduct]                                                                                   |              [✅][CurtainList]               |         [✅][CurtainStatus]          |         [✅][CurtainWebhook]          |         [✅][CurtainCommand]          |      ✅       |
| [Curtain3][Curtain3Product]  [[JP][Curtain3ProductJP]]                                                      |              [✅][Curtain3List]              |         [✅][Curtain3Status]         |         [✅][Curtain3Webhook]         |         [✅][Curtain3Command]         |              |
| [Blind Tilt][BlindTiltProduct]  [[JP][BlindTiltProductJP]]                                                  |             [✅][BlindTiltList]              |        [✅][BlindTiltStatus]         |                  -                   |        [✅][BlindTiltCommand]         |              |
| [Universal Remote][UniversalRemoteProduct]  [[JP][UniversalRemoteProductJP]]                                |                      -                      |                                     |                                      |                                      |      📦      |
| Wallet Finder Card  [[JP][WalletFinderCardProductJP]]                                                       |                      -                      |                                     |                                      |                                      |      📦      |
| **Home Appliance**                                                                                          |                      -                      |                  -                  |                  -                   |                  -                   |      -       |
| [Humidifier][HumidifierProduct]  [[JP][HumidifierProductJP]]                                                |             [✅][HumidifierList]             |        [✅][HumidifierStatus]        |                  -                   |        [✅][HumidifierCommand]        |              |
| Evaporative Humidifier  [[JP][EvaporativeHumidifierProductJP]]                                              |                      -                      |                                     |                                      |                                      |              |
| Evaporative Humidifier Plus  [[JP][EvaporativeHumidifierPlusProductJP]]                                     |                      -                      |                                     |                                      |                                      |              |
| Fan                                                                                                         |                      -                      |                                     |                                      |                                      |              |
| [Battery Circulator Fan][BatteryCirculatorFanProduct]  [[JP][BatteryCirculatorFanProductJP]]                |        [✅][BatteryCirculatorFanList]        |   [✅][BatteryCirculatorFanStatus]   |   [✅][BatteryCirculatorFanWebhook]   |   [✅][BatteryCirculatorFanCommand]   |              |
| Circulator Fan Lite  [[JP][CirculatorFanLiteProductJP]]                                                     |                      -                      |                  ✅                  |                  ✅                   |                  ✅                   |      📦      |
| **Robot Vacuum**                                                                                            |                      -                      |                  -                  |                  -                   |                  -                   |      -       |
| Robot Vacuum Cleaner S1  [[JP][RobotVacuumCleanerS1ProductJP]]                                              |        [✅][RobotVacuumCleanerS1List]        |   [✅][RobotVacuumCleanerS1Status]   |   [✅][RobotVacuumCleanerS1Webhook]   |   [✅][RobotVacuumCleanerS1Command]   |              |
| Robot Vacuum Cleaner S1 Plus  [[JP][RobotVacuumCleanerS1PlusProductJP]]                                     |      [✅][RobotVacuumCleanerS1PlusList]      | [✅][RobotVacuumCleanerS1PlusStatus] | [✅][RobotVacuumCleanerS1PlusWebhook] | [✅][RobotVacuumCleanerS1PlusCommand] |              |
| [Mini Robot Vacuum K10+][MiniRobotVacuumK10+Product]  [[JP][MiniRobotVacuumK10+ProductJP]]                  |                      -                      |                                     |                                      |                                      |              |
| [Mini Robot Vacuum K10+ Pro][MiniRobotVacuumK10+ProProduct]  [[JP][MiniRobotVacuumK10+ProProductJP]]        |                      -                      |                                     |                                      |                                      |              |
| [Floor Cleaning Robot S10][FloorCleaningRobotS10Product]  [[JP][FloorCleaningRobotS10ProductJP]]            |       [✅][FloorCleaningRobotS10List]        |  [✅][FloorCleaningRobotS10Status]   |  [✅][FloorCleaningRobotS10Webhook]   |  [✅][FloorCleaningRobotS10Command]   |              |
| **Sensor**                                                                                                  |                      -                      |                  -                  |                  -                   |                  -                   |      -       |
| [Meter][MeterProduct]  [[JP][MeterProductJP]]                                                               |               [✅][MeterList]                |          [✅][MeterStatus]           |          [✅][MeterWebhook]           |                  -                   |              |
| [Meter Plus][MeterPlusProduct]  [[JP][MeterPlusProductJP]]                                                  |             [✅][MeterPlusList]              |        [✅][MeterPlusStatus]         |        [✅][MeterPlusWebhook]         |                  -                   |      ✅       |
| Meter Pro                                                                                                   |                      -                      |                                     |                                      |                                      |              |
| [Outdoor Meter][OutdoorMeterProduct]  [[JP][OutdoorMeterProductJP]]                                         |            [✅][OutdoorMeterList]            |       [✅][OutdoorMeterStatus]       |       [✅][OutdoorMeterWebhook]       |                  -                   |              |
| [Motion Sensor][MotionSensorProduct]  [[JP][MotionSensorProductJP]]                                         |            [✅][MotionSensorList]            |       [✅][MotionSensorStatus]       |       [✅][MotionSensorWebhook]       |                  -                   |      ✅       |
| [Contact Sensor][ContactSensorProduct]  [[JP][ContactSensorProductJP]]                                      |           [✅][ContactSensorList]            |      [✅][ContactSensorStatus]       |      [✅][ContactSensorWebhook]       |                  -                   |              |
| [Water Leak Detector][WaterLeakDetectorProduct]  [[JP][WaterLeakDetectorProductJP]]                         |         [✅][WaterLeakDetectorList]          |    [✅][WaterLeakDetectorStatus]     |    [✅][WaterLeakDetectorWebhook]     |                  -                   |              |
| **Security(Lock)**                                                                                          |                      -                      |                  -                  |                  -                   |                  -                   |      -       |
| [Smart Lock][SmartLockProduct]  [[JP][SmartLockProductJP]]                                                  |             [✅][SmartLockList]              |        [✅][SmartLockStatus]         |        [✅][SmartLockWebhook]         |        [✅][SmartLockCommand]         |      ✅       |
| [Smart Lock Pro][SmartLockProProduct]  [[JP][SmartLockProProductJP]]                                        |            [✅][SmartLockProList]            |       [✅][SmartLockProStatus]       |       [✅][SmartLockProWebhook]       |       [✅][SmartLockProCommand]       |              |
| Keypad [JP][KeypadProductJP]                                                                                |               [✅][KeypadList]               |          [✅][KeypadStatus]          |          [✅][KeypadWebhook]          |          [✅][KeypadCommand]          |              |
| [Keypad Touch][KeypadTouchProduct]  [[JP][KeypadTouchProductJP]]                                            |            [✅][KeypadTouchList]             |       [✅][KeypadTouchStatus]        |       [✅][KeypadTouchWebhook]        |       [✅][KeypadTouchCommand]        |      ✅       |
| **Security(Camera)**                                                                                        |                      -                      |                  -                  |                  -                   |                  -                   |      -       |
| [Outdoor Spotlight Cam 1080P][OutdoorSpotlightCam1080PProduct]  [[JP][OutdoorSpotlightCam1080PProductJP]]   |                      -                      |                                     |                                      |                                      |              |
| [Outdoor Spotlight Cam 2K(3MP)][OutdoorSpotlightCam2K3MPProduct]  [[JP][OutdoorSpotlightCam2K3MPProductJP]] |                      -                      |                                     |                                      |                                      |      📦      |
| [Pan/Tilt Cam][PanTiltCamProduct]  [[JP][PanTiltCamProductJP]]                                              |             [✅][PanTiltCamList]             |                  -                  |        [✅][PanTiltCamWebhook]        |                  -                   |              |
| [Pan/Tilt Cam 2K(3MP)][PanTiltCam2K3MPProduct]  [[JP][PanTiltCam2K3MPProductJP]]                            |          [✅][PanTiltCam2K3MPList]           |                                     |                                      |                                      |              |
| Pan/Tilt Cam Plus(3MP)  [[JP][PanTiltCamPlus3MPProductJP]]                                                  |                      -                      |                                     |                                      |                                      |              |
| Pan/Tilt Cam Plus(5MP)  [[JP][PanTiltCamPlus5MPProductJP]]                                                  |                      -                      |                                     |                                      |                                      |      📦      |
| [Indoor Cam][IndoorCamProduct]  [[JP][IndoorCamProductJP]]                                                  |             [✅][IndoorCamList]              |                  -                  |        [✅][IndoorCamWebhook]         |                  -                   |      📦      |
| **Power & Switch**                                                                                          |                      -                      |                  -                  |                  -                   |                  -                   |      -       |
| Plug                                                                                                        |                [✅][PlugList]                |           [✅][PlugStatus]           |                  -                   |           [✅][PlugCommand]           |              |
| [Plug Mini (US)][PlugMiniUSProduct]                                                                         |             [✅][PlugMiniUSList]             |        [✅][PlugMiniUSStatus]        |        [✅][PlugMiniUSWebhook]        |        [✅][PlugMiniUSCommand]        |              |
| Plug Mini (JP)  [[JP][PlugMiniJPProductJP]]                                                                 |             [✅][PlugMiniJPList]             |        [✅][PlugMiniJPStatus]        |        [✅][PlugMiniJPWebhook]        |        [✅][PlugMiniJPCommand]        |      ✅       |
| [Remote][RemoteProduct]  [[JP][RemoteProductJP]]                                                            |               [✅][RemoteList]               |                  -                  |                  -                   |                  -                   |      -       |
| **Lighting**                                                                                                |                      -                      |                  -                  |                  -                   |                  -                   |      -       |
| Ceiling Light  [[JP][CeilingLightProductJP]]                                                                |            [✅][CeilingLightList]            |       [✅][CeilingLightStatus]       |       [✅][CeilingLightWebhook]       |       [✅][CeilingLightCommand]       |              |
| Ceiling Light Pro  [[JP][CeilingLightProProductJP]]                                                         |          [✅][CeilingLightProList]           |     [✅][CeilingLightProStatus]      |     [✅][CeilingLightProWebhook]      |     [✅][CeilingLightProCommand]      |              |
| [Color Bulb][ColorBulbProduct]  [[JP][ColorBulbProductJP]]                                                  |             [✅][ColorBulbList]              |        [✅][ColorBulbStatus]         |        [✅][ColorBulbWebhook]         |        [✅][ColorBulbCommand]         |      ✅       |
| [Strip Light][StripLightProduct]  [[JP][StripLightProductJP]]                                               |             [✅][StripLightList]             |        [✅][StripLightStatus]        |        [✅][StripLightWebhook]        |        [✅][StripLightCommand]        |              |

[GetDeviceList]: https://github.com/OpenWonderLabs/SwitchBotAPI#get-device-list
[StatusAPI]: https://github.com/OpenWonderLabs/SwitchBotAPI#get-device-status
[Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#webhook
[CommandAPI]: https://github.com/OpenWonderLabs/SwitchBotAPI#get-device-status
[HubList]: https://github.com/OpenWonderLabs/SwitchBotAPI#hubhub-plushub-minihub-2
[HubPlusList]: https://github.com/OpenWonderLabs/SwitchBotAPI#hubhub-plushub-minihub-2
[HubMiniProduct]: https://www.switch-bot.com/products/switchbot-hub-mini
[HubMiniProductJP]: https://www.switchbot.jp/products/switchbot-hub-mini
[HubMiniList]: https://github.com/OpenWonderLabs/SwitchBotAPI#hubhub-plushub-minihub-2
[Hub2Product]: https://www.switch-bot.com/pages/switchbot-hub-2
[Hub2ProductJP]: https://www.switchbot.jp/products/switchbot-hub2
[Hub2List]: https://github.com/OpenWonderLabs/SwitchBotAPI#hubhub-plushub-minihub-2
[Hub2Status]: https://github.com/OpenWonderLabs/SwitchBotAPI#hub-2
[Hub2Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#hub-2-1
[BotProduct]: https://www.switch-bot.com/products/switchbot-bot
[BotProductJP]: https://www.switchbot.jp/products/switchbot-bot
[BotList]: https://github.com/OpenWonderLabs/SwitchBotAPI#bot
[BotStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#bot-1
[BotWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#bot-3
[BotCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#bot-2
[CurtainProduct]: https://www.switch-bot.com/products/switchbot-curtain
[CurtainList]: https://github.com/OpenWonderLabs/SwitchBotAPI#curtain
[CurtainStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#curtain-1
[CurtainWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#curtain-4
[CurtainCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#curtain-2
[Curtain3Product]: https://www.switch-bot.com/products/switchbot-curtain-3
[Curtain3ProductJP]: https://www.switchbot.jp/products/switchbot-curtain3
[Curtain3List]: https://github.com/OpenWonderLabs/SwitchBotAPI#curtain-3
[Curtain3Status]: https://github.com/OpenWonderLabs/SwitchBotAPI#curtain-3-1
[Curtain3Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#curtain-3-3
[Curtain3Command]: https://github.com/OpenWonderLabs/SwitchBotAPI#curtain-3-2
[BlindTiltProduct]: https://www.switch-bot.com/products/switchbot-blind-tilt
[BlindTiltProductJP]: https://www.switchbot.jp/products/switchbot-blind-tilt
[BlindTiltList]: https://github.com/OpenWonderLabs/SwitchBotAPI#blind-tilt
[BlindTiltStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#blind-tilt-1
[BlindTiltCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#blind-tilt-2
[UniversalRemoteProduct]: https://www.switch-bot.com/products/switchbot-universal-remote
[UniversalRemoteProductJP]: https://www.switchbot.jp/products/switchbot-universal-remote
[WalletFinderCardProductJP]: https://www.switchbot.jp/products/switchbot-wallet-finder-card
[HumidifierProduct]: https://www.switch-bot.com/products/switchbot-smart-humidifier
[HumidifierProductJP]: https://www.switchbot.jp/products/switchbot-smart-humidifier?variant=40981225799855
[HumidifierList]: https://github.com/OpenWonderLabs/SwitchBotAPI#humidifier
[HumidifierStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#humidifier-1
[HumidifierCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#humidifier-2
[EvaporativeHumidifierProductJP]: https://www.switchbot.jp/products/switchbot-evaporative-humidifier
[EvaporativeHumidifierPlusProductJP]: https://www.switchbot.jp/products/switchbot-evaporative-humidifier-plus
[BatteryCirculatorFanProduct]: https://us.switch-bot.com/products/switchbot-battery-circulator-fan
[BatteryCirculatorFanProductJP]: https://www.switchbot.jp/products/switchbot-smart-circulator-fan?variant=44020075167919
[BatteryCirculatorFanList]: https://github.com/OpenWonderLabs/SwitchBotAPI#battery-circulator-fan
[BatteryCirculatorFanStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#battery-circulator-fan-1
[BatteryCirculatorFanWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#battery-circulator-fan-3
[BatteryCirculatorFanCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#battery-circulator-fan-2
[CirculatorFanLiteProductJP]: https://www.switchbot.jp/products/switchbot-smart-circulator-fan?variant=44221010182319
[RobotVacuumCleanerS1ProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner?variant=41850919420079
[RobotVacuumCleanerS1List]: https://github.com/OpenWonderLabs/SwitchBotAPI#robot-vacuum-cleaner-s1
[RobotVacuumCleanerS1Status]: https://github.com/OpenWonderLabs/SwitchBotAPI#robot-vacuum-cleaner-s1-1
[RobotVacuumCleanerS1Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#robot-vacuum-cleaner-s1-3
[RobotVacuumCleanerS1Command]: https://github.com/OpenWonderLabs/SwitchBotAPI#robot-vacuum-cleaner-s1-2
[RobotVacuumCleanerS1PlusProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner?variant=44254800347311
[RobotVacuumCleanerS1PlusList]: https://github.com/OpenWonderLabs/SwitchBotAPI#robot-vacuum-cleaner-s1-plus
[RobotVacuumCleanerS1PlusStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#robot-vacuum-cleaner-s1-plus-1
[RobotVacuumCleanerS1PlusWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#robot-vacuum-cleaner-s1-plus-3
[RobotVacuumCleanerS1PlusCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#robot-vacuum-cleaner-s1-plus-2
[MiniRobotVacuumK10+Product]: https://www.switch-bot.com/products/switchbot-mini-robot-vacuum-k10
[MiniRobotVacuumK10+ProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner-k10
[MiniRobotVacuumK10+ProProduct]: https://www.switch-bot.com/products/switchbot-mini-robot-vacuum-k10-pro
[MiniRobotVacuumK10+ProProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner-k10-pro
[FloorCleaningRobotS10Product]: https://www.switch-bot.com/products/switchbot-floor-cleaning-robot-s10
[FloorCleaningRobotS10ProductJP]: https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner-s10
[FloorCleaningRobotS10List]: https://github.com/OpenWonderLabs/SwitchBotAPI#floor-cleaning-robot-s10
[FloorCleaningRobotS10Status]: https://github.com/OpenWonderLabs/SwitchBotAPI#floor-cleaning-robot-s10-1
[FloorCleaningRobotS10Webhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#floor-cleaning-robot-s10-3
[FloorCleaningRobotS10Command]: https://github.com/OpenWonderLabs/SwitchBotAPI#floor-cleaning-robot-s10-2
[MeterProduct]: https://www.switch-bot.com/products/switchbot-meter
[MeterProductJP]: https://www.switchbot.jp/products/switchbot-meter
[MeterList]: https://github.com/OpenWonderLabs/SwitchBotAPI#meter
[MeterStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#meter-1
[MeterWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#meter-2
[MeterPlusProduct]: https://www.switch-bot.com/products/switchbot-meter-plus
[MeterPlusProductJP]: https://www.switchbot.jp/products/switchbot-meter-plus
[MeterPlusList]: https://github.com/OpenWonderLabs/SwitchBotAPI#meter-plus
[MeterPlusStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#meter-plus-1
[MeterPlusWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#meter-plus-2
[OutdoorMeterProduct]: https://www.switch-bot.com/products/switchbot-indoor-outdoor-thermo-hygrometer
[OutdoorMeterProductJP]: https://www.switchbot.jp/products/switchbot-indoor-outdoor-meter
[OutdoorMeterList]: https://github.com/OpenWonderLabs/SwitchBotAPI#outdoor-meter
[OutdoorMeterStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#outdoor-meter-1
[OutdoorMeterWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#outdoor-meter-2
[MotionSensorProduct]: https://www.switch-bot.com/products/motion-sensor
[MotionSensorProductJP]: https://www.switchbot.jp/products/switchbot-motion-sensor
[MotionSensorList]: https://github.com/OpenWonderLabs/SwitchBotAPI#motion-sensor
[MotionSensorStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#motion-sensor-1
[MotionSensorWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#motion-sensor-2
[ContactSensorProduct]: https://www.switch-bot.com/products/contact-sensor
[ContactSensorProductJP]: https://www.switchbot.jp/products/switchbot-contact-sensor
[ContactSensorList]: https://github.com/OpenWonderLabs/SwitchBotAPI#contact-sensor
[ContactSensorStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#contact-sensor-1
[ContactSensorWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#contact-sensor-2
[WaterLeakDetectorProduct]: https://www.switch-bot.com/products/switchbot-water-leak-detector
[WaterLeakDetectorProductJP]: https://www.switchbot.jp/products/switchbot-water-leak-detector
[WaterLeakDetectorList]: https://github.com/OpenWonderLabs/SwitchBotAPI#water-leak-detector
[WaterLeakDetectorStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#water-leak-detector-1
[WaterLeakDetectorWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#water-leak-detector-2
[SmartLockProduct]: https://www.switch-bot.com/products/switchbot-lock
[SmartLockProductJP]: https://www.switchbot.jp/products/switchbot-lock
[SmartLockList]: https://github.com/OpenWonderLabs/SwitchBotAPI#lock
[SmartLockStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#lock-1
[SmartLockWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#lock-3
[SmartLockCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#lock-2
[SmartLockProProduct]: https://www.switch-bot.com/products/switchbot-lock-pro
[SmartLockProProductJP]: https://www.switchbot.jp/products/switchbot-lock-pro
[SmartLockProList]: https://github.com/OpenWonderLabs/SwitchBotAPI#lock-pro
[SmartLockProStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#lock-pro-1
[SmartLockProWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#lock-pro-3
[SmartLockProCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#lock-pro-2
[KeypadProductJP]: https://www.switchbot.jp/products/switchbot-keypad
[KeypadList]: https://github.com/OpenWonderLabs/SwitchBotAPI#keypad
[KeypadStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#keypad-1
[KeypadWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#keypad-3
[KeypadCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#keypad-2
[KeypadTouchProduct]: https://switch-bot.com/pages/switchbot-keypad
[KeypadTouchProductJP]: https://www.switchbot.jp/products/switchbot-keypad-touch
[KeypadTouchList]: https://github.com/OpenWonderLabs/SwitchBotAPI#keypad-touch
[KeypadTouchStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#keypad-touch-1
[KeypadTouchWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#keypad-touch-3
[KeypadTouchCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#keypad-touch-2
[OutdoorSpotlightCam1080PProduct]: https://www.switch-bot.com/products/switchbot-outdoor-spotlight-cam?variant=43002833338535
[OutdoorSpotlightCam1080PProductJP]: https://www.switchbot.jp/products/switchbot-outdoor-spotlight-cam
[OutdoorSpotlightCam2K3MPProduct]: https://www.switch-bot.com/products/switchbot-outdoor-spotlight-cam?variant=45882280738983
[OutdoorSpotlightCam2K3MPProductJP]: https://www.switchbot.jp/products/switchbot-outdoor-spotlight-cam-3mp
[PanTiltCamProduct]: https://switch-bot.com/pages/switchbot-pan-tilt-cam
[PanTiltCamProductJP]: https://www.switchbot.jp/products/switchbot-pan-tilt-cam
[PanTiltCamList]: https://github.com/OpenWonderLabs/SwitchBotAPI#pantilt-cam
[PanTiltCamWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#pantilt-cam-1
[PanTiltCam2K3MPProduct]: https://switch-bot.com/pages/switchbot-pan-tilt-cam-2k
[PanTiltCam2K3MPProductJP]: https://www.switchbot.jp/products/switchbot-pan-tilt-cam-3mp
[PanTiltCam2K3MPList]: https://github.com/OpenWonderLabs/SwitchBotAPI#pantilt-cam-2k
[PanTiltCamPlus3MPProductJP]: https://www.switchbot.jp/products/switchbot-pan-tilt-cam-plus-3mp
[PanTiltCamPlus5MPProductJP]: https://www.switchbot.jp/products/switchbot-pan-tilt-cam-plus-5mp
[IndoorCamProduct]: https://switch-bot.com/pages/switchbot-indoor-cam
[IndoorCamProductJP]: https://www.switchbot.jp/collections/security-camera-solution/products/switchbot-indoor-cam
[IndoorCamList]: https://github.com/OpenWonderLabs/SwitchBotAPI#indoor-cam
[IndoorCamWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#indoor-cam-1
[PlugList]: https://github.com/OpenWonderLabs/SwitchBotAPI#plug
[PlugStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#plug-1
[PlugCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#plug-2
[PlugMiniUSProduct]: https://switch-bot.com/pages/switchbot-plug-mini
[PlugMiniUSList]: https://github.com/OpenWonderLabs/SwitchBotAPI#plug-mini-us
[PlugMiniUSStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#plug-mini-us-1
[PlugMiniUSWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#plug-mini-us-3
[PlugMiniUSCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#plug-mini-us-2
[PlugMiniJPProductJP]: https://www.switchbot.jp/products/switchbot-plug-mini
[PlugMiniJPList]: https://github.com/OpenWonderLabs/SwitchBotAPI#plug-mini-jp
[PlugMiniJPStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#plug-mini-jp-1
[PlugMiniJPWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#plug-mini-jp-3
[PlugMiniJPCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#plug-mini-jp-2
[RemoteProduct]: https://switch-bot.com/products/switchbot-remote
[RemoteProductJP]: https://www.switchbot.jp/products/switchbot-remote
[RemoteList]: https://github.com/OpenWonderLabs/SwitchBotAPI#remote
[CeilingLightProductJP]: https://www.switchbot.jp/products/switchbot-ceiling-light?variant=42442788438191
[CeilingLightList]: https://github.com/OpenWonderLabs/SwitchBotAPI#ceiling-light
[CeilingLightStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#ceiling-light-1
[CeilingLightWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#ceiling-light-3
[CeilingLightCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#ceiling-light-2
[CeilingLightProProductJP]: https://www.switchbot.jp/products/switchbot-ceiling-light?variant=42442788503727
[CeilingLightProList]: https://github.com/OpenWonderLabs/SwitchBotAPI#ceiling-light-pro
[CeilingLightProStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#ceiling-light-pro-1
[CeilingLightProWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#ceiling-light-pro-3
[CeilingLightProCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#ceiling-light-pro-2
[ColorBulbProduct]: https://www.switch-bot.com/products/switchbot-color-bulb
[ColorBulbProductJP]: https://www.switchbot.jp/products/switchbot-color-bulb
[ColorBulbList]: https://github.com/OpenWonderLabs/SwitchBotAPI#color-bulb
[ColorBulbStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#color-bulb-1
[ColorBulbWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#color-bulb-3
[ColorBulbCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#color-bulb-2
[StripLightProduct]: https://www.switch-bot.com/products/switchbot-light-strip
[StripLightProductJP]: https://www.switchbot.jp/products/switchbot-strip-light
[StripLightList]: https://github.com/OpenWonderLabs/SwitchBotAPI#strip-light
[StripLightStatus]: https://github.com/OpenWonderLabs/SwitchBotAPI#strip-light-1
[StripLightWebhook]: https://github.com/OpenWonderLabs/SwitchBotAPI#led-strip-light
[StripLightCommand]: https://github.com/OpenWonderLabs/SwitchBotAPI#strip-light-2

### Virtual Infrared Remote Devices

| Device               | [Command](https://github.com/OpenWonderLabs/SwitchBotAPI#command-set-for-virtual-infrared-remote-devices) |
|----------------------|:-------:|
| Air Conditioner      |    ✅    |
| TV                   |    ✅    |
| Light                |    ✅    |
| Fan                  |    ✅    |
| IPTV(Streamer)       |    ✅    |
| Set Top Box          |    ✅    |
| DVD Player           |    ✅    |
| Speaker              |    ✅    |
| Robot Vacuum Cleaner |    ✅    |
| Air Purifier         |    ✅    |
| Water Heater(Bath)   |    ✅    |
| Projector            |    ✅    |
| Camera               |    ✅    |
| Others               |    ✅    |

## Installation Method

For installation instructions, please refer to [here](https://github.com/hsakoh/switchbot-mqtt/wiki).
