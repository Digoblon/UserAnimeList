using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace UserAnimeList.Communication.JsonConverters;

public partial class SanitizedStringConverter: JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()?.Trim();

        return value is null ? null : RemoveExtraWhiteSpaces().Replace(value, " ");
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) => writer.WriteStringValue(value);
    
    [GeneratedRegex(@"\s+")]
    private static partial Regex RemoveExtraWhiteSpaces();
}
