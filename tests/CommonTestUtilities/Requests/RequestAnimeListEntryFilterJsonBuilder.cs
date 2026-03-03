using Bogus;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestAnimeListEntryFilterJsonBuilder
{
    public static RequestAnimeListEntryFilterJson Build()
    {
        return new Faker<RequestAnimeListEntryFilterJson>()
            .RuleFor(x => x.Query, f => f.Lorem.Word())
            .RuleFor(x => x.Status, f => f.PickRandom<AnimeEntryStatus>());
    }
}