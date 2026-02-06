using UserAnimeList.Communication.Enums;

namespace UserAnimeList.Communication.Requests;

public class RequestAnimeListEntryJson
{
    public Guid AnimeId { get; set; }
    public AnimeEntryStatus Status { get; set; } = AnimeEntryStatus.Watching;
    public int? Score { get; set; }
    public int? Progress { get; set; }
    public DateOnly? DateStarted { get; set; }
    public DateOnly? DateFinished { get; set; }
}