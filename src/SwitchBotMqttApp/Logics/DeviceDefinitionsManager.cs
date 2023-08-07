using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using SwitchBotMqttApp.Models.DeviceDefinitions;
using SwitchBotMqttApp.Models.Enums;
using SwitchBotMqttApp.Models.HomeAssistant;
using System.Globalization;
using System.Text;

namespace SwitchBotMqttApp.Logics;

public class DeviceDefinitionsManager
{
    public DeviceDefinitionsManager()
    {
        {
            using var reader = new StreamReader(Path.Combine(AppContext.BaseDirectory, "MasterData","DeviceDefinition.csv"), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });
            csv.Context.TypeConverterCache.AddConverter<DeviceType>(new EnumConverter<DeviceType>());
            csv.Context.TypeConverterCache.AddConverter<PhysicalOrVirtual>(new EnumConverter<PhysicalOrVirtual>());
            csv.Context.RegisterClassMap<DeviceDefinitionMap>();
            csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("");
            DeviceDefinitions = csv.GetRecords<DeviceDefinition>().ToList().AsReadOnly();
        }
        {
            using var reader = new StreamReader(Path.Combine(AppContext.BaseDirectory, "MasterData","FieldDefinition.csv"), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });
            csv.Context.TypeConverterCache.AddConverter<DeviceType>(new EnumConverter<DeviceType>());
            csv.Context.TypeConverterCache.AddConverter<FieldSourceType>(new EnumConverter<FieldSourceType>());
            csv.Context.TypeConverterCache.AddConverter<FieldDataType>(new EnumConverter<FieldDataType>());
            csv.Context.TypeConverterCache.AddConverter<SensorDeviceClass>(new EnumConverter<SensorDeviceClass>());
            csv.Context.TypeConverterCache.AddConverter<BinarySensorDeviceClass>(new EnumConverter<BinarySensorDeviceClass>());
            csv.Context.RegisterClassMap<FieldDefinitionMap>();
            csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("");
            FieldDefinitions = csv.GetRecords<FieldDefinition>().ToList().AsReadOnly();
        }
        {
            using var reader = new StreamReader(Path.Combine(AppContext.BaseDirectory, "MasterData","CommandDefinition.csv"), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });
            csv.Context.TypeConverterCache.AddConverter<DeviceType>(new EnumConverter<DeviceType>());
            csv.Context.TypeConverterCache.AddConverter<CommandType>(new EnumConverter<CommandType>());
            csv.Context.TypeConverterCache.AddConverter<PayloadType>(new EnumConverter<PayloadType>());
            csv.Context.TypeConverterCache.AddConverter<ButtonDeviceClass>(new EnumConverter<ButtonDeviceClass>());
            csv.Context.RegisterClassMap<CommandDefinitionMap>();
            csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("");
            CommandDefinitions = csv.GetRecords<CommandDefinition>().ToList().AsReadOnly();
        }
        {
            using var reader = new StreamReader(Path.Combine(AppContext.BaseDirectory, "MasterData","CommandPayloadDefinition.csv"), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });
            csv.Context.TypeConverterCache.AddConverter<DeviceType>(new EnumConverter<DeviceType>());
            csv.Context.TypeConverterCache.AddConverter<CommandType>(new EnumConverter<CommandType>());
            csv.Context.TypeConverterCache.AddConverter<ParameterType>(new EnumConverter<ParameterType>());
            csv.Context.TypeConverterCache.AddConverter<NumberDeviceClass>(new EnumConverter<NumberDeviceClass>());
            csv.Context.TypeConverterCache.AddConverter<NumberMode>(new EnumConverter<NumberMode>());
            csv.Context.RegisterClassMap<CommandPayloadDefinitionMap>();
            csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("");
            CommandPayloadDefinitions = csv.GetRecords<CommandPayloadDefinition>().ToList().AsReadOnly();
        }
        {
            using var reader = new StreamReader(Path.Combine(AppContext.BaseDirectory, "MasterData","KeySetDefinition.csv"), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });
            csv.Context.TypeConverterCache.AddConverter<DeviceType>(new EnumConverter<DeviceType>());
            csv.Context.RegisterClassMap<KeySetDefinitionMap>();
            csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("");
            KeySetDefinitions = csv.GetRecords<KeySetDefinition>().ToList().AsReadOnly();
        }
    }

    public IReadOnlyList<DeviceDefinition> DeviceDefinitions { get; set; }
    public IReadOnlyList<FieldDefinition> FieldDefinitions { get; set; }
    public IReadOnlyList<CommandDefinition> CommandDefinitions { get; set; }
    public IReadOnlyList<CommandPayloadDefinition> CommandPayloadDefinitions { get; set; }
    public IReadOnlyList<KeySetDefinition> KeySetDefinitions { get; set; }
    private class DeviceDefinitionMap : ClassMap<DeviceDefinition>
    {
        public DeviceDefinitionMap()
        {
            Map(m => m.DeviceType).Index(0).TypeConverter<EnumConverter<DeviceType>>();
            Map(m => m.ApiDeviceTypeString).Index(1);
            Map(m => m.CustomizedDeviceTypeString).Index(2);
            Map(m => m.WebhookDeviceTypeString).Index(3);
            Map(m => m.PhysicalOrVirtual).Index(4).TypeConverter<EnumConverter<PhysicalOrVirtual>>();
            Map(m => m.IsSupportPolling).Index(5);
            Map(m => m.IsSupportWebhook).Index(6);
            Map(m => m.IsSupportCommand).Index(7);
            Map(m => m.Description).Index(8);
            Map(m => m.DisplayName).Index(9);
            Map(m => m.DisplayNameJa).Index(10);
        }
    }
    private class FieldDefinitionMap : ClassMap<FieldDefinition>
    {
        public FieldDefinitionMap()
        {
            Map(m => m.DeviceType).Index(0).TypeConverter<EnumConverter<DeviceType>>();
            Map(m => m.FieldName).Index(1);
            Map(m => m.FieldSourceType).Index(2).TypeConverter<EnumConverter<FieldSourceType>>();
            Map(m => m.StatusKey).Index(3);
            Map(m => m.WebhookKey).Index(4);
            Map(m => m.FieldDataType).Index(5).TypeConverter<EnumConverter<FieldDataType>>();
            Map(m => m.Description).Index(6);
            Map(m => m.IsBinary).Index(7);
            Map(m => m.BinarySensorDeviceClass).Index(8).TypeConverter<EnumConverter<BinarySensorDeviceClass>>();
            Map(m => m.OnValue).Index(9);
            Map(m => m.OffValue).Index(10);
            Map(m => m.Icon).Index(11);
            Map(m => m.SensorDeviceClass).Index(12).TypeConverter<EnumConverter<SensorDeviceClass>>();
            Map(m => m.EntityCategory).Index(13);
            Map(m => m.StateClass).Index(14);
            Map(m => m.UnitOfMeasurement).Index(15);
            Map(m => m.DisplayName).Index(16);
            Map(m => m.DisplayNameJa).Index(17);
        }
    }
    private class CommandDefinitionMap : ClassMap<CommandDefinition>
    {
        public CommandDefinitionMap()
        {
            Map(m => m.DeviceType).Index(0).TypeConverter<EnumConverter<DeviceType>>();
            Map(m => m.CommandType).Index(1).TypeConverter<EnumConverter<CommandType>>();
            Map(m => m.Command).Index(2);
            Map(m => m.PayloadType).Index(3).TypeConverter<EnumConverter<PayloadType>>();
            Map(m => m.Description).Index(4);
            Map(m => m.Icon).Index(5);
            Map(m => m.ButtonDeviceClass).Index(6).TypeConverter<EnumConverter<ButtonDeviceClass>>();
            Map(m => m.DisplayName).Index(7);
            Map(m => m.DisplayNameJa).Index(8);
        }
    }

    private class CommandPayloadDefinitionMap : ClassMap<CommandPayloadDefinition>
    {
        public CommandPayloadDefinitionMap()
        {
            Map(m => m.DeviceType).Index(0).TypeConverter<EnumConverter<DeviceType>>();
            Map(m => m.CommandType).Index(1).TypeConverter<EnumConverter<CommandType>>();
            Map(m => m.Command).Index(2);
            Map(m => m.Name).Index(3);
            Map(m => m.Index).Index(4);
            Map(m => m.Description).Index(5);
            Map(m => m.ParameterType).Index(6).TypeConverter<EnumConverter<ParameterType>>();
            Map(m => m.Icon).Index(7);
            Map(m => m.Options).Index(8);
            Map(m => m.OptionsDescription).Index(9);
            Map(m => m.RangeMin).Index(10);
            Map(m => m.RangeMax).Index(11);
            Map(m => m.NumberDeviceClass).Index(12).TypeConverter<EnumConverter<NumberDeviceClass>>();
            Map(m => m.NumberMode).Index(13).TypeConverter<EnumConverter<NumberMode>>();
            Map(m => m.UnitOfMeasurement).Index(14);
            Map(m => m.LengthMin).Index(15);
            Map(m => m.LengthMax).Index(16);
            Map(m => m.DefaultValue).Index(17);
            Map(m => m.DisplayName).Index(18);
            Map(m => m.DisplayNameJa).Index(19);
        }
    }

    private class KeySetDefinitionMap : ClassMap<KeySetDefinition>
    {
        public KeySetDefinitionMap()
        {
            Map(m => m.DeviceType).Index(0).TypeConverter<EnumConverter<DeviceType>>();
            Map(m => m.KeyName).Index(1);
            Map(m => m.KeyTag).Index(2);

        }
    }

    public class EnumConverter<T> : EnumConverter where T : struct
    {
        public EnumConverter() : base(typeof(T))
        {
        }

        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (Enum.TryParse<T>(text, out var result))
            {
                return result;
            }
            return null;
        }
    }
}
