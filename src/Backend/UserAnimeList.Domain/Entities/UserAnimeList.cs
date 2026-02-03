using UserAnimeList.Domain.Enums;

namespace UserAnimeList.Domain.Entities;

public class UserAnimeList : EntityBase
{
    public Guid AnimeId { get; set; }
    public Guid UserId { get; set; }
    public AnimeEntryStatus Status { get; set; } = AnimeEntryStatus.Watching;
    public int? Score { get; set; }
    public int? Progress { get; set; }
    public DateOnly? DateStarted { get; set; }
    public DateOnly? DateFinished { get; set; }
    public User User { get; set; } = null!;
    public Anime Anime { get; set; } = null!;
}