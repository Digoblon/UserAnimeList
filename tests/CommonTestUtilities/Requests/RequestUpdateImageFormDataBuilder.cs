using Bogus;
using Microsoft.AspNetCore.Http;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestUpdateImageFormDataBuilder
{
    public static RequestUpdateImageFormData Build(IFormFile? formFile = null)
    {
        return new Faker<RequestUpdateImageFormData>()
            .RuleFor(x => x.Image, _ => formFile);
    }
}