using Bogus;
using UserAnimeList.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class AnimeListBuilder
{
    public static IList<AnimeList> Collection(User user,uint count = 2)
    {
        
        var list = new List<AnimeList>();

        if (count == 0)
            count = 1;

        for (int i = 0; i < count; i++)
        {
            var anime = AnimeBuilder.Build();
            var fakeAnimeList = Build(anime, user);
            
            list.Add(fakeAnimeList);
        }
        
        return list;
    }
    
    public static AnimeList Build(Anime anime, User user)
    {
        return new Faker<AnimeList>()
            .RuleFor(l => l.Id, Guid.NewGuid())
            .RuleFor(l => l.AnimeId, anime.Id)
            .RuleFor(l => l.UserId, user.Id)
            .RuleFor(l => l.Anime, anime)
            .RuleFor(l => l.User, user)
            .RuleFor(l => l.Score, faker => faker.Random.Int(1, 10))
            .RuleFor(l => l.Progress, f => f.Random.Int(1, anime.Episodes!.Value))
            .RuleFor(l => l.DateStarted, (f) => f.Date.PastDateOnly());
    }
}