using System.Text.Json.Serialization;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Communication.JsonConverters;

namespace UserAnimeList.Communication.Requests;

public class RequestAnimeListEntryFilterJson
{
    [JsonConverter(typeof(SanitizedStringConverter))]
    public string? Query { get; set; }
    public AnimeEntryStatus?  Status { get; set; }
    public DateOnly? DateStarted { get; set; }
    public DateOnly? DateFinished { get; set; }
    public AnimeListSort? SortField { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.Asc;
}