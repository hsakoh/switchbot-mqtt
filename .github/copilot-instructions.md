# GitHub Copilot Instructions for switchbot-mqtt

## Project Overview

This project is a **Home Assistant Add-on** that bridges SwitchBot devices with MQTT, enabling integration with Home Assistant. It is a .NET 8 Blazor Server application that:

- Communicates with SwitchBot Cloud API to control physical and virtual infrared remote devices
- Publishes device states and accepts commands via MQTT protocol
- Provides a web UI for device configuration and management
- Supports both polling and webhook-based device state updates
- Integrates with Home Assistant MQTT Discovery for automatic device setup

## Architecture

### Projects Structure

1. **SwitchBotMqttApp** (`src/SwitchBotMqttApp/`)
   - Main Blazor Server application
   - Ports: 8098 (Webhook), 8099 (Web UI)
   - Contains device management logic, MQTT services, and web UI

2. **HomeAssistantAddOn.Mqtt** (`src/HomeAssistant/HomeAssistantAddOn.Mqtt/`)
   - MQTT client library for Home Assistant integration
   - Handles MQTT broker connection and message publishing

3. **HomeAssistantAddOn.Core** (`src/HomeAssistant/HomeAssistantAddOn.Core/`)
   - Core utilities for Home Assistant Add-on functionality
   - Configuration management

### Key Components

#### Services
- **MqttCoreService**: Manages MQTT connections and message publishing/subscribing
- **PollingService**: Polls SwitchBot API periodically to get device status updates
- **WebhookService**: Handles webhook registration and real-time device event updates
- **AutomatedHostedService**: Coordinates service lifecycle and startup sequence

#### Managers
- **DeviceConfigurationManager**: Manages device configurations (physical and virtual devices)
- **DeviceDefinitionsManager**: Loads and manages device type definitions and capabilities
- **DeviceStatePersistanceManager**: Persists device state across application restarts

#### API Client
- **SwitchBotApiClient**: Handles authentication and communication with SwitchBot Cloud API (v1.1)

## Technology Stack

- **.NET 8**: Target framework
- **Blazor Server**: Interactive web UI with SignalR
- **MQTTnet**: MQTT client library
- **Blazored.Modal**: Modal dialog support
- **FluffySpoon.Ngrok**: Ngrok integration for webhook tunneling
- **Home Assistant MQTT Discovery**: Auto-configuration protocol

## Coding Guidelines

### General Principles

1. **Follow existing patterns**: This codebase has established patterns for services, managers, and models
2. **Async/await**: All I/O operations should be asynchronous with proper CancellationToken support
3. **Dependency Injection**: Use constructor injection for all dependencies
4. **Logging**: Use structured logging with appropriate log levels
5. **Configuration**: Use Options pattern for configuration management

### Naming Conventions

- **Services**: Suffix with `Service` (e.g., `MqttCoreService`, `PollingService`)
- **Managers**: Suffix with `Manager` (e.g., `DeviceConfigurationManager`)
- **Options**: Suffix with `Options` (e.g., `SwitchBotOptions`, `WebhookServiceOptions`)
- **Models**: Use descriptive names without suffixes unless needed for clarity
- **MQTT entities**: Suffix with `Config` (e.g., `ButtonConfig`, `SensorConfig`)

### Project-Specific Patterns

#### Configuration Management
```csharp
// Use IOptions<T> pattern for configuration
public class MyService
{
    private readonly IOptions<MyOptions> _options;
    
    public MyService(IOptions<MyOptions> options)
    {
        _options = options;
    }
}
```

#### Device Models
- **PhysicalDevice**: Represents actual SwitchBot hardware
- **VirtualInfraredRemoteDevice**: Represents IR remote controls
- Device definitions are loaded from JSON files in device-definitions directory

#### MQTT Message Patterns
- Config topic: `homeassistant/{component}/{objectId}/config`
- Command topic: `switchbot-mqtt/{deviceId}/command`
- State topic: `switchbot-mqtt/{deviceId}/state`
- Use Home Assistant MQTT Discovery format

#### API Rate Limiting
- SwitchBot API has rate limits (10,000 calls/day)
- Track API call counts with `ApiCallCount` dictionary
- Prefer webhook updates over polling when available

### Error Handling

1. **API Errors**: Handle SwitchBot API status codes appropriately
2. **MQTT Errors**: Implement reconnection logic
3. **Logging**: Log exceptions with context (device ID, operation type, etc.)
4. **User Feedback**: Show meaningful error messages in the UI

### Testing Considerations

1. **Mock SwitchBot API**: Use HttpClient factory for testability
2. **Debug proxy**: Development mode includes Fiddler proxy support (localhost:8888)
3. **Ngrok support**: For webhook testing in development environment

## Device Support

### Physical Devices
Support for SwitchBot devices including:
- Bot, Curtain, Hub, Plug, Lock
- Sensors (Motion, Contact, Meter, Indoor/Outdoor Meter)
- Lights (Color Bulb, Strip Light, RGBICWW variants)
- Humidifiers, Fans, Blinds, etc.

### Virtual Infrared Remote Devices
Support for IR remotes controlling:
- Air Conditioners, TVs, Lights, Fans
- Projectors, Speakers, Cameras
- And more (see device-definitions JSON files)

## Home Assistant Integration

### MQTT Discovery
- Automatically creates Home Assistant entities
- Supports multiple entity types: button, sensor, binary_sensor, number, text, select
- Device classes map to Home Assistant conventions

### Configuration Flow
1. User configures SwitchBot API credentials
2. Application discovers devices via API
3. User selects which devices/features to expose
4. MQTT Discovery messages are published
5. Entities appear in Home Assistant

## Important Files to Understand

- `Program.cs`: Application startup and DI configuration
- `appsettings.json`: Configuration structure (DO NOT commit secrets)
- `Models/DeviceDefinitions/`: Device capability definitions
- `Services/MqttCoreService.cs`: Core MQTT integration logic
- `Logics/SwitchBotApiClient.cs`: SwitchBot API communication

## Security Notes

1. **API Credentials**: Never hardcode API keys or secrets
2. **HMAC Authentication**: SwitchBot API requires proper signature generation
3. **Data Protection**: Keys are persisted using ASP.NET Core Data Protection
4. **Home Assistant Ingress**: Support for HA's reverse proxy authentication

## Common Tasks

### Adding a New Device Type
1. Update device-definitions JSON with capabilities
2. Add enum values to `DeviceType` or `RemoteDeviceType`
3. Update `DeviceDefinitionsManager` if new field types needed
4. Test with actual device or API documentation

### Adding a New Command
1. Define in device-definitions JSON under `Commands` array
2. Specify CommandType, PayloadType, and parameters
3. Update UI if new payload type required
4. Test command execution via `SwitchBotApiClient`

### Modifying MQTT Topics
1. Update topic patterns in respective `*Config` classes
2. Ensure compatibility with Home Assistant MQTT Discovery spec
3. Update documentation if user-facing topics change

## Resources

- [SwitchBot API Documentation](https://github.com/OpenWonderLabs/SwitchBotAPI)
- [Home Assistant MQTT Discovery](https://www.home-assistant.io/integrations/mqtt/#mqtt-discovery)
- [Project Wiki](https://github.com/hsakoh/switchbot-mqtt/wiki) - Installation and usage guides

## Development Workflow

1. Use Blazor hot reload for UI development
2. Check logs at INFO level during development
3. Use modal dialogs for testing commands before MQTT integration
4. Validate JSON configurations before deployment
5. Test with multiple device types when possible

---

**Note**: This project targets Home Assistant Add-on deployment but can run standalone with proper MQTT broker configuration.
