using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestUpdateStudioJsonBuilder
{
    public static RequestUpdateStudioJson Build()
    {
        var request = new Faker<RequestUpdateStudioJson>()
            .RuleFor(user => user.Name, (f) => f.Company.CompanyName())
            .RuleFor(user => user.Description, (f, u) => f.Lorem.Paragraph());

        return request;
    }
}