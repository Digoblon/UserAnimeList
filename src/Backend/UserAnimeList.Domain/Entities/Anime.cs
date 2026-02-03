using UserAnimeList.Domain.Enums;
namespace UserAnimeList.Domain.Entities;

public class Anime : EntityBase
{ 
    public string Name { get; set; } =  string.Empty;
    public string NameNormalized { get; set; } = string.Empty;
    public string Synopsis { get; set; }  =  string.Empty;
    public int? Episodes { get; set; }
    public ICollection<AnimeGenre> Genres { get; set; } = [];
    public ICollection<AnimeStudio> Studios { get; set; } = [];
    public AnimeStatus Status { get; set; }
    public SourceType Source { get; set; }
    public AnimeType Type { get; set; }
    public DateOnly? AiredFrom { get; set; }
    public DateOnly? AiredUntil { get; set; }
    public ICollection<UserAnimeList> UserEntries = [];
    public (Season Season, int Year)? Premiered
    {
        get
        {
            if (AiredFrom is null)
                return null;

            var date = AiredFrom.Value;

            var season = date.Month switch
            {
                >= 1 and <= 3 => Season.Winter,
                >= 4 and <= 6 => Season.Spring,
                >= 7 and <= 9 => Season.Summer,
                >= 10 and <= 12 => Season.Fall,
                _ => throw new ArgumentOutOfRangeException()
            };

            return (season, date.Year);
        }
    }
}