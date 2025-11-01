using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using SwitchBotMqttApp.Models.DeviceDefinitions;
using SwitchBotMqttApp.Models.Enums;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace SwitchBotMqttApp.Logics;

public class DeviceDefinitionsManager
{
    public DeviceDefinitionsManager()
    {
        {
            using var reader = new StreamReader(Path.Combine(AppContext.BaseDirectory, "MasterData", "KeySetDefinition.csv"), Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });
            csv.Context.TypeConverterCache.AddConverter<DeviceType>(new EnumConverter<DeviceType>());
            csv.Context.RegisterClassMap<KeySetDefinitionMap>();
            csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("");
            KeySetDefinitions = csv.GetRecords<KeySetDefinition>().ToList().AsReadOnly();
        }
        {
            List<DeviceDefinition> deviceDefinitions = [];
            var basePath = Path.Combine(AppContext.BaseDirectory, "MasterData");
            foreach (var filePath in Directory.GetFiles(Path.Combine(basePath, nameof(PhysicalOrVirtual.PhysicalDevice))))
            {
                deviceDefinitions.Add(JsonSerializer.Deserialize<DeviceDefinition>(File.ReadAllText(filePath, Encoding.UTF8))!);
            }
            foreach (var filePath in Directory.GetFiles(Path.Combine(basePath, nameof(PhysicalOrVirtual.VirtualInfraredRemoteDevice))))
            {
                deviceDefinitions.Add(JsonSerializer.Deserialize<DeviceDefinition>(File.ReadAllText(filePath, Encoding.UTF8))!);
            }
            DeviceDefinitions = deviceDefinitions.ToList().AsReadOnly();
        }
    }

    public IReadOnlyList<DeviceDefinition> DeviceDefinitions { get; set; }
    public IReadOnlyList<KeySetDefinition> KeySetDefinitions { get; set; }

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
