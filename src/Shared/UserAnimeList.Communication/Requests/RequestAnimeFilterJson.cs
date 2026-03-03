using System.Text.Json.Serialization;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Communication.JsonConverters;

namespace UserAnimeList.Communication.Requests;

public class RequestAnimeFilterJson
{
    [JsonConverter(typeof(SanitizedStringConverter))]
    public string? Query { get; set; }

    public AnimeStatus? Status { get; set; }
    public AnimeType? Type { get; set; }

    public IList<Guid>? Genres { get; set; }
    public IList<Guid>? Studios { get; set; }

    public DateOnly? AiredFrom { get; set; }
    public DateOnly? AiredUntil { get; set; }

    public Season? PremieredSeason { get; set; }
    public int? PremieredYear { get; set; }

    public AnimeSort? SortField { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.Asc;
}