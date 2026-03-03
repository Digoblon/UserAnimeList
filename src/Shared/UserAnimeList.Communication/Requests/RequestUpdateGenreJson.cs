using System.Text.Json.Serialization;
using UserAnimeList.Communication.JsonConverters;

namespace UserAnimeList.Communication.Requests;

public class RequestUpdateGenreJson
{
    [JsonConverter(typeof(SanitizedStringConverter))]
    public string Name { get; set; } =  string.Empty;
    [JsonConverter(typeof(SanitizedStringConverter))]
    public string Description { get; set; } = string.Empty;
}