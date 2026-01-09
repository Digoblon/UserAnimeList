using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestLoginJsonBuilder
{
    public static RequestLoginJson Build()
    {
        return new Faker<RequestLoginJson>()
            .RuleFor(u => u.Login, (f) => f.Internet.Email())
            .RuleFor(u => u.Password, (f) => f.Internet.Password());
    }
}