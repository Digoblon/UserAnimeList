using System.Text.RegularExpressions;
using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public partial class RequestUpdateUserJsonBuilder
{
    public static RequestUpdateUserJson Build()
    {
        return new Faker<RequestUpdateUserJson>()
            .RuleFor(user => user.UserName, (f) => SanitizeUserName(f.Internet.UserName()))
            .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.UserName));
    }
    
    private static string SanitizeUserName(string userName)
    {
        return MyRegex().Replace(userName, "");
    }

    [GeneratedRegex("[^a-zA-Z0-9_-]")]
    private static partial Regex MyRegex();
}