using UserAnimeList.Domain.Enums;

namespace UserAnimeList.Infrastructure.Data.Seed.DTOs;

//CLASSE GERADA POR INTELIGÊNCIA ARTIFICIAL
public sealed class SeedAnimeListEntryJson
{
    public string UserEmail { get; set; } = string.Empty;
    public string AnimeName { get; set; } = string.Empty;

    public AnimeEntryStatus Status { get; set; } = AnimeEntryStatus.Watching;
    public int? Score { get; set; }
    public int? Progress { get; set; }
    public DateOnly? DateStarted { get; set; }
    public DateOnly? DateFinished { get; set; }
}