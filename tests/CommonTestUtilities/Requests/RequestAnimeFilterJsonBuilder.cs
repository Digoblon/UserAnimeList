using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestAnimeFilterJsonBuilder
{
    public static RequestAnimeFilterJson Build()
    {
        return new Faker<RequestAnimeFilterJson>()
            .RuleFor(f => f.Query, f => f.Lorem.Word());
    }
}