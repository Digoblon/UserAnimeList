namespace UserAnimeList.Domain.Entities;

public class Genre : EntityBase
    {
        public string Name { get; set; } = string.Empty;
        public string NameNormalized { get; set; } = string.Empty;
        public string Description { get; set; } =  string.Empty;
        
        public ICollection<AnimeGenre> Animes { get; set; } = new List<AnimeGenre>();
    }