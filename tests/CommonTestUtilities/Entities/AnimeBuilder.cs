using Bogus;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class AnimeBuilder
{
    public static IList<Anime> Collection(uint count = 2)
    {
        var list = new List<Anime>();

        if (count == 0)
            count = 1;

        for (int i = 0; i < count; i++)
        {
            var fakeAnime = Build();
            
            list.Add(fakeAnime);
        }
        
        return list;
    }
    
    public static Anime Build()
    {
        return new Faker<Anime>()
            .RuleFor(a => a.Id, f=> Guid.NewGuid())
            .RuleFor(a => a.Name, f => f.Lorem.Word())
            .RuleFor(a => a.NameNormalized, (_, a) => a.Name.ToLower())
            .RuleFor(a => a.ImagePath, f => f.Image.PicsumUrl())
            .RuleFor(a => a.Synopsis, f => f.Lorem.Paragraph())
            .RuleFor(a => a.Episodes, f => f.Random.Int(1, 100))
            .RuleFor(a => a.Status, f => f.PickRandom<AnimeStatus>())
            .RuleFor(a => a.Source, f => f.PickRandom<SourceType>())
            .RuleFor(a => a.Type, f => f.PickRandom<AnimeType>())
            .RuleFor(a => a.AiredFrom, f => f.Date.PastDateOnly())
            .RuleFor(a => a.AiredUntil, f => f.Date.FutureDateOnly())
            .RuleFor(a => a.Studios,(_, a) => CreateAnimeStudio(a.Id))
            .RuleFor(a => a.Genres,(_, a) => CreateAnimeGenre(a.Id));
    }

    private static IList<AnimeStudio> CreateAnimeStudio(Guid animeId)
    {
        var studioList = new List<AnimeStudio>();

        studioList.Add(new AnimeStudio { StudioId = Guid.NewGuid(), AnimeId = animeId });
        studioList.Add(new AnimeStudio { StudioId = Guid.NewGuid(), AnimeId = animeId });
        studioList.Add(new AnimeStudio { StudioId = Guid.NewGuid(), AnimeId = animeId });
        
        return studioList;
    }
    
    private static IList<AnimeGenre> CreateAnimeGenre(Guid animeId)
    {
        var genreList = new List<AnimeGenre>();

        genreList.Add(new AnimeGenre { GenreId = Guid.NewGuid(), AnimeId = animeId });
        genreList.Add(new AnimeGenre { GenreId = Guid.NewGuid(), AnimeId = animeId });
        genreList.Add(new AnimeGenre { GenreId = Guid.NewGuid(), AnimeId = animeId });
        
        return genreList;
    }
}