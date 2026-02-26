using PatientDemo.Domain.Attributes;

namespace PatientDemo.Application.Common.Extensions;

/// <summary>
/// Класс с методами расширения для Enum
/// </summary>
public static class EnumExtensions
{
    public static string? GetLocalizedDescription(this Enum? @enum)
    {
        if (@enum == null)
            return null;

        var description = @enum.ToString();

        var fieldInfo = @enum.GetType().GetField(description);
        var attributes = (LocalizedDescriptionAttribute[])fieldInfo!.GetCustomAttributes(typeof(LocalizedDescriptionAttribute), false);

        return attributes.Any() ? attributes[0].Description : description;
    }
}