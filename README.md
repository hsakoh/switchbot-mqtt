# SwitchBot MQTT Home Assistant add-on

This project is a Home Assistant add-on that allows you to control various SwitchBot products through the API.

The add-on can also receive Webhooks to obtain the device's status.
Via an MQTT broker, it will be detected as an MQTT integration in Home Assistant.

You can perform manual scene executions that were configured in the SwitchBot app, as well as control virtual infrared remote devices.

**Important: Please note that this add-on does not support operations on SwitchBot devices via Bluetooth.**

## Current Support Status

![aarch64-shield](https://img.shields.io/badge/aarch64-yes-green.svg)
![amd64-shield](https://img.shields.io/badge/amd64-yes-green.svg)

### Physical Devices

We have implemented all devices according to the published API specifications, but testing has been conducted only on a subset.

| Device                                                                                           | Status | Webhook | Command | Verification |
|--------------------------------------------------------------------------------------------------|:------:|:-------:|:-------:|:------------:|
| [Bot](https://www.switch-bot.com/products/switchbot-bot)                                         |   ✅    |    ✅    |    ✅    |              |
| [Curtain](https://www.switch-bot.com/products/switchbot-curtain)                                 |   ✅    |    ✅    |    ✅    |      ✅       |
| Hub                                                                                              |   -    |    -    |    -    |              |
| Hub Plus                                                                                         |   -    |    -    |    -    |              |
| [Hub Mini](https://www.switch-bot.com/products/switchbot-hub-mini)                               |   -    |    -    |    -    |              |
| [Hub 2](https://us.switch-bot.com/pages/switchbot-hub-2)                                         |   ✅    |    ✅    |    -    |      ✅       |
| [Meter](https://www.switch-bot.com/products/switchbot-meter)                                     |   ✅    |    ✅    |    -    |              |
| [MeterPlus](https://www.switch-bot.com/products/switchbot-meter-plus)                            |   ✅    |    ✅    |    -    |              |
| [Outdoor Meter](https://www.switch-bot.com/products/switchbot-indoor-outdoor-thermo-hygrometer)  |   ✅    |    ✅    |    -    |              |
| [Smart Lock](https://us.switch-bot.com/products/switchbot-lock)                                  |   ✅    |    ✅    |    ✅    |      ✅       |
| Keypad                                                                                           |   ✅    |    ✅    |    ✅    |              |
| [Keypad Touch](https://switch-bot.com/pages/switchbot-keypad)                                    |   ✅    |    ✅    |    ✅    |              |
| [Remote](https://switch-bot.com/products/switchbot-remote)                                       |   -    |    -    |    -    |      -       |
| [Motion Sensor](https://www.switch-bot.com/products/motion-sensor)                               |   ✅    |    ✅    |    -    |      ✅       |
| [Contact Sensor](https://www.switch-bot.com/products/contact-sensor)                             |   ✅    |    ✅    |    -    |              |
| [Ceiling Light](https://www.switchbot.jp/collections/all/products/switchbot-ceiling-light)       |   ✅    |    ✅    |    ✅    |              |
| [Ceiling Light Pro](https://www.switchbot.jp/collections/all/products/switchbot-ceiling-light)   |   ✅    |    ✅    |    ✅    |              |
| [Plug Mini (US)](https://switch-bot.com/pages/switchbot-plug-mini)                               |   ✅    |    ✅    |    ✅    |              |
| [Plug Mini (JP)](https://www.switchbot.jp/products/switchbot-plug-mini)                          |   ✅    |    ✅    |    ✅    |      ✅       |
| Plug                                                                                             |   ✅    |    -    |    ✅    |              |
| [Strip Light](https://www.switch-bot.com/products/switchbot-light-strip)                         |   ✅    |    ✅    |    ✅    |              |
| [Color Bulb](https://www.switch-bot.com/products/switchbot-color-bulb)                           |   ✅    |    ✅    |    ✅    |      ✅       |
| [Robot Vacuum Cleaner S1](https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner)      |   ✅    |    ✅    |    ✅    |              |
| [Robot Vacuum Cleaner S1 Plus](https://www.switchbot.jp/products/switchbot-robot-vacuum-cleaner) |   ✅    |    ✅    |    ✅    |              |
| [Humidifier](https://www.switch-bot.com/products/switchbot-smart-humidifier)                     |   ✅    |    -    |    ✅    |              |
| [Indoor Cam](https://switch-bot.com/pages/switchbot-indoor-cam)                                  |   -    |    ✅    |    -    |              |
| [Pan/Tilt Cam](https://switch-bot.com/pages/switchbot-pan-tilt-cam-2k)                           |   -    |    ✅    |    -    |              |
| [Blind Tilt](https://us.switch-bot.com/pages/switchbot-blind-tilt)                               |   ✅    |    -    |    ✅    |              |

### Virtual Infrared Remote Devices

| Device          | Command |
|-----------------|:-------:|
| Air Conditioner |    ✅    |
| TV              |    ✅    |
| IPTV            |    ✅    |
| Set Top Box     |    ✅    |
| DVD             |    ✅    |
| Fan             |    ✅    |
| Projector       |    ✅    |
| Camera          |    ✅    |
| Others          |    ✅    |
| Air Purifier    |    ✅    |
| Speaker         |    ✅    |
| Water Heater    |    ✅    |
| Vacuum Cleaner  |    ✅    |
| Light           |    ✅    |

## Installation Method

For installation instructions, please refer to [here](INSTALLATION.md).
