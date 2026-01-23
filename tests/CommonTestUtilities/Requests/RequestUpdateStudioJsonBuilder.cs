using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestUpdateStudioJsonBuilder
{
    public static RequestUpdateStudioJson Build()
    {
        var request = new Faker<RequestUpdateStudioJson>()
            .RuleFor(studio => studio.Name, (f) => f.Company.CompanyName())
            .RuleFor(studio => studio.Description, (f, u) => f.Lorem.Paragraph());

        return request;
    }
}