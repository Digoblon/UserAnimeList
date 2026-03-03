using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestRegisterStudioJsonBuilder
{
    public static RequestRegisterStudioJson Build()
    {
        var request = new Faker<RequestRegisterStudioJson>()
            .RuleFor(studio => studio.Name, (f) => f.Company.CompanyName())
            .RuleFor(studio => studio.Description, (f, u) => f.Lorem.Paragraph());

        return request;
    }
}