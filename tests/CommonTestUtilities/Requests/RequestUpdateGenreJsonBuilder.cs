using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestUpdateGenreJsonBuilder
{
    public static RequestUpdateGenreJson Build()
    {
        var request = new Faker<RequestUpdateGenreJson>()
            .RuleFor(user => user.Name, (f) => f.Lorem.Word())
            .RuleFor(user => user.Description, (f, u) => f.Lorem.Paragraph());

        return request;
    }
}