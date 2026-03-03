using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestAnimeListEntryJsonBuilder
{
    public static RequestAnimeListEntryJson Build()
    {
        return new Faker<RequestAnimeListEntryJson>()
            .RuleFor(l => l.AnimeId, Guid.NewGuid())
            .RuleFor(l => l.Score, faker => faker.Random.Int(1, 10))
            .RuleFor(l => l.Progress, 0)
            .RuleFor(l => l.DateStarted, (f) => f.Date.PastDateOnly());
    }
}