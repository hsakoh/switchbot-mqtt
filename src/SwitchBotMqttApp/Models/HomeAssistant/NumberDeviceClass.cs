using System.Runtime.Serialization;

namespace SwitchBotMqttApp.Models.HomeAssistant;

public enum NumberDeviceClass
{
    /// <summary>
    /// Generic number. This is the default and doesn’t need to be set.
    /// </summary>

    None,
    /// <summary>
    /// Apparent power in VA.
    /// </summary>
    [EnumMember(Value = "apparent_power")]
    ApparentPower,
    /// <summary>
    /// Air Quality Index (unitless).
    /// </summary>
    [EnumMember(Value = "aqi")]
    Aqi,
    /// <summary>
    /// Atmospheric pressure in cbar, bar, hPa, inHg, kPa, mbar, Pa, psi
    /// </summary>
    [EnumMember(Value = "atmospheric_pressure")]
    AtmosphericPressure,
    /// <summary>
    /// Percentage of battery that is left
    /// </summary>
    [EnumMember(Value = "battery")]
    Battery,
    /// <summary>
    /// Carbon Dioxide in CO2 (Smoke)
    /// </summary>
    [EnumMember(Value = "carbon_dioxide")]
    CarbonDioxide,
    /// <summary>
    /// Carbon Monoxide in CO (Gas CNG/LPG)
    /// </summary>
    [EnumMember(Value = "carbon_monoxide")]
    CarbonMonoxide,
    /// <summary>
    /// Current in A, mA
    /// </summary>
    [EnumMember(Value = "current")]
    Current,
    /// <summary>
    /// Data rate in bit/s, kbit/s, Mbit/s, Gbit/s, B/s, kB/s, MB/s, GB/s, KiB/s, MiB/s, or GiB/s
    /// </summary>
    [EnumMember(Value = "data_rate")]
    DataRate,
    /// <summary>
    /// Data size in bit, kbit, Mbit, Gbit, B, kB, MB, GB, TB, PB, EB, ZB, YB, KiB, MiB, GiB, TiB, PiB, EiB, ZiB, or YiB
    /// </summary>
    [EnumMember(Value = "data_size")]
    DataSize,
    /// <summary>
    /// Generic distance in km, m, cm, mm, mi, yd, or in
    /// </summary>
    [EnumMember(Value = "distance")]
    Distance,
    /// <summary>
    /// Energy in Wh, kWh, MWh, MJ, or GJ
    /// </summary>
    [EnumMember(Value = "energy")]
    Energy,
    /// <summary>
    /// Stored energy in Wh, kWh, MWh, MJ, or GJ
    /// </summary>
    [EnumMember(Value = "energy_storage")]
    EnergyStorage,
    /// <summary>
    /// Frequency in Hz, kHz, MHz, or GHz
    /// </summary>
    [EnumMember(Value = "frequency")]
    Frequency,
    /// <summary>
    /// Gasvolume in m³, ft³, or CCF
    /// </summary>
    [EnumMember(Value = "gas")]
    Gas,
    /// <summary>
    /// Percentage of humidity in the air
    /// </summary>
    [EnumMember(Value = "humidity")]
    Humidity,
    /// <summary>
    /// The current light level in lx
    /// </summary>
    [EnumMember(Value = "illuminance")]
    Illuminance,
    /// <summary>
    /// Irradiance in W/m² or BTU/(h⋅ft²)
    /// </summary>
    [EnumMember(Value = "irradiance")]
    Irradiance,
    /// <summary>
    /// Percentage of water in a substance
    /// </summary>
    [EnumMember(Value = "moisture")]
    Moisture,
    /// <summary>
    /// The monetary value
    /// </summary>
    [EnumMember(Value = "monetary")]
    Monetary,
    /// <summary>
    /// Concentration of Nitrogen Dioxide in µg/m³
    /// </summary>
    [EnumMember(Value = "nitrogen_dioxide")]
    NitrogenDioxide,
    /// <summary>
    /// Concentration of Nitrogen Monoxide in µg/m³
    /// </summary>
    [EnumMember(Value = "nitrogen_monoxide")]
    NitrogenMonoxide,
    /// <summary>
    /// Concentration of Nitrous Oxide in µg/m³
    /// </summary>
    [EnumMember(Value = "nitrous_oxide")]
    NitrousOxide,
    /// <summary>
    /// Concentration of Ozone in µg/m³
    /// </summary>
    [EnumMember(Value = "ozone")]
    Ozone,
    /// <summary>
    /// Potential hydrogen (pH) value of a water solution
    /// </summary>
    [EnumMember(Value = "ph")]
    Ph,
    /// <summary>
    /// Concentration of particulate matter less than 1 micrometer in µg/m³
    /// </summary>
    [EnumMember(Value = "pm1")]
    Pm1,
    /// <summary>
    /// Concentration of particulate matter less than 10 micrometers in µg/m³
    /// </summary>
    [EnumMember(Value = "pm10")]
    Pm10,
    /// <summary>
    /// Concentration of particulate matter less than 2.5 micrometers in µg/m³
    /// </summary>
    [EnumMember(Value = "pm25")]
    Pm25,
    /// <summary>
    /// Power factor(unitless), unit may be None or %
    /// </summary>
    [EnumMember(Value = "power_factor")]
    PowerFactor,
    /// <summary>
    /// Power in W or kW
    /// </summary>
    [EnumMember(Value = "power")]
    Power,
    /// <summary>
    /// Accumulated precipitation in cm, in or mm
    /// </summary>
    [EnumMember(Value = "precipitation")]
    Precipitation,
    /// <summary>
    /// Precipitation intensity in in/d, in/h, mm/d, or mm/h
    /// </summary>
    [EnumMember(Value = "precipitation_intensity")]
    PrecipitationIntensity,
    /// <summary>
    /// Pressure in Pa, kPa, hPa, bar, cbar, mbar, mmHg, inHg, or psi
    /// </summary>
    [EnumMember(Value = "pressure")]
    Pressure,
    /// <summary>
    /// Reactive power in var
    /// </summary>
    [EnumMember(Value = "reactive_power")]
    ReactivePower,
    /// <summary>
    /// Signal strength in dB or dBm
    /// </summary>
    [EnumMember(Value = "signal_strength")]
    SignalStrength,
    /// <summary>
    /// Sound pressure in dB or dBA
    /// </summary>
    [EnumMember(Value = "sound_pressure")]
    SoundPressure,
    /// <summary>
    /// Generic speed in ft/s, in/d, in/h, km/h, kn, m/s, mph, or mm/d
    /// </summary>
    [EnumMember(Value = "speed")]
    Speed,
    /// <summary>
    /// Concentration of sulphur dioxide in µg/m³
    /// </summary>
    [EnumMember(Value = "sulphur_dioxide")]
    SulphurDioxide,
    /// <summary>
    /// Temperature in °C, °F or K
    /// </summary>
    [EnumMember(Value = "temperature")]
    Temperature,
    /// <summary>
    /// Concentration of volatile organic compounds in µg/m³
    /// </summary>
    [EnumMember(Value = "volatile_organic_compounds")]
    VolatileOrganicCompounds,
    /// <summary>
    /// Voltage in V, mV
    /// </summary>
    [EnumMember(Value = "voltage")]
    Voltage,
    /// <summary>
    /// Generic volume in L, mL, gal, fl. oz., m³, ft³, or CCF
    /// </summary>
    [EnumMember(Value = "volume")]
    Volume,
    /// <summary>
    /// Generic stored volume in L, mL, gal, fl. oz., m³, ft³, or CCF
    /// </summary>
    [EnumMember(Value = "volume_storage")]
    VolumeStorage,
    /// <summary>
    /// Water consumption in L, gal, m³, ft³, or CCF
    /// </summary>
    [EnumMember(Value = "water")]
    Water,
    /// <summary>
    /// Generic mass in kg, g, mg, µg, oz, lb, or st
    /// </summary>
    [EnumMember(Value = "weight")]
    Weight,
    /// <summary>
    /// Wind speed in ft/s, km/h, kn, m/s, or mph
    /// </summary>
    [EnumMember(Value = "wind_speed")]
    WindSpeed,
}