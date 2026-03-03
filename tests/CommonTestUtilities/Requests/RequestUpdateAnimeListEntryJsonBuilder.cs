using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestUpdateAnimeListEntryJsonBuilder
{
    public static RequestUpdateAnimeListEntryJson Build()
    {
        return new Faker<RequestUpdateAnimeListEntryJson>()
            .RuleFor(l => l.Score, faker => faker.Random.Int(1, 10))
            .RuleFor(l => l.Progress, 0)
            .RuleFor(l => l.DateStarted, (f) => f.Date.PastDateOnly());
    }
}