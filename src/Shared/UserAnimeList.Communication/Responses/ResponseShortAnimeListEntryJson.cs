using UserAnimeList.Communication.Enums;

namespace UserAnimeList.Communication.Responses;

public class ResponseShortAnimeListEntryJson
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AnimeEntryStatus Status { get; set; } = AnimeEntryStatus.Watching;
    public int? Score { get; set; }
    public int? Progress { get; set; }
}