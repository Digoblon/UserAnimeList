using UserAnimeList.Domain.Enums;

namespace UserAnimeList.Infrastructure.Data.Seed.DTOs;

//CLASSE GERADA POR INTELIGÊNCIA ARTIFICIAL
public sealed class SeedAnimeJson
{
    public string Name { get; set; } = string.Empty;
    public string? Synopsis { get; set; }
    public int? Episodes { get; set; }

    public List<string> Genres { get; set; } = [];
    public List<string> Studios { get; set; } = [];

    public AnimeStatus Status { get; set; }
    public SourceType Source { get; set; }
    public AnimeType Type { get; set; }

    public DateOnly? AiredFrom { get; set; }
    public DateOnly? AiredUntil { get; set; }
}