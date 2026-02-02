namespace UserAnimeList.Domain.Entities;

public class AnimeStudio
{
    public Guid AnimeId { get; set; }
    public Anime Anime { get; set; } = null!;

    public Guid StudioId { get; set; }
    public Studio Studio { get; set; } = null!;
}