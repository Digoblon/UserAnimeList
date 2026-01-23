using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestRegisterGenreJsonBuilder
{
    public static RequestRegisterGenreJson Build()
    {
        var request = new Faker<RequestRegisterGenreJson>()
            .RuleFor(genre => genre.Name, (f) => f.Company.CompanyName())
            .RuleFor(genre => genre.Description, (f, u) => f.Lorem.Paragraph());

        return request;
    }
}