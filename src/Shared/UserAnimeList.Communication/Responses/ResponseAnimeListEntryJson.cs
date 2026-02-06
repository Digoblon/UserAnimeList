using UserAnimeList.Communication.Enums;

namespace UserAnimeList.Communication.Responses;

public class ResponseAnimeListEntryJson
{
    public Guid Id { get; set; }
    public Guid AnimeId { get; set; }
    public AnimeEntryStatus Status { get; set; } = AnimeEntryStatus.Watching;
    public int? Score { get; set; }
    public int? Progress { get; set; }
    public DateOnly? DateStarted { get; set; }
    public DateOnly? DateFinished { get; set; }
    
}