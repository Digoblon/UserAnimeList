using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestRegisterStudioJsonBuilder
{
    public static RequestRegisterStudioJson Build()
    {
        var request = new Faker<RequestRegisterStudioJson>()
            .RuleFor(user => user.Name, (f) => f.Company.CompanyName())
            .RuleFor(user => user.Description, (f, u) => f.Lorem.Paragraph());

        return request;
    }
}