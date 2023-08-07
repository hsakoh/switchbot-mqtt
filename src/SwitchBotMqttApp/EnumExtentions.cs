using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;

namespace SwitchBotMqttApp;


public static class EnumExtentions
{
    public static string? ToDisplayName(this Enum value)
    {
        return value?
            .GetType()
            .GetField(value.ToString())?
            .GetCustomAttribute<DisplayAttribute>(false)?
            .GetName();
    }
    public static string? ToEnumMemberValue(this Enum value)
    {
        return value?
            .GetType()
            .GetField(value.ToString())?
            .GetCustomAttribute<EnumMemberAttribute>(false)?
            .Value;
    }
    public static TEnum ToEnumFromDisplayName<TEnum>(this string value) where TEnum : Enum
    {
        foreach (var val in Enum.GetValues(typeof(TEnum)))
        {
            if (typeof(TEnum).GetField(val.ToString()!)!.GetCustomAttribute<DisplayAttribute>()?.GetName() == value)
            {
                return (TEnum)val;
            }
        }
        return default!;
    }
    public static TEnum? ToEnumFromEnumMember<TEnum>(this string value) where TEnum : struct
    {
        foreach (var val in Enum.GetValues(typeof(TEnum)))
        {
            if (typeof(TEnum).GetField(val.ToString()!)!.GetCustomAttribute<EnumMemberAttribute>()?.Value == value)
            {
                return (TEnum)val;
            }
        }
        return null;
    }
}
