using System.Text.Json.Serialization;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Communication.JsonConverters;

namespace UserAnimeList.Communication.Requests;

public class RequestAnimeJson
{
    [JsonConverter(typeof(SanitizedStringConverter))]
    public string Name { get; set; } =  string.Empty;
    
    [JsonConverter(typeof(SanitizedStringConverter))]
    public string? Synopsis { get; set; } =  string.Empty;
    
    public int? Episodes { get; set; }
    
    public IList<Guid> Genres { get; set; } = [];
    public IList<Guid> Studios { get; set; } = [];
    public AnimeStatus Status { get; set; }
    public SourceType Source { get; set; }
    public AnimeType Type { get; set; }
    public DateOnly? AiredFrom { get; set; }
    public DateOnly? AiredUntil { get; set; }
}