using System.Text.Json;
using System.Text.Json.Serialization;

namespace PatientDemo.Shared.JsonConverters;

/// <summary>
/// Конвертер пустой строки, пробельной строки и т.п. в null
/// </summary>
public class EmptyStringToNullConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        return string.IsNullOrWhiteSpace(str) ? null : str;
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value ?? string.Empty);
    }
}