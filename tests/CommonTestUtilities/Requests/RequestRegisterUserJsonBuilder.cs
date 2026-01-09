using System.Text.RegularExpressions;
using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public partial class RequestRegisterUserJsonBuilder
{
    public static RequestRegisterUserJson Build(int passwordLength = 10)
    {
        var request =  new Faker<RequestRegisterUserJson>()
            .RuleFor(user => user.UserName, (f) => SanitizeUserName(f.Internet.UserName()))
            .RuleFor(user => user.Email, (f, u) => f.Internet.Email(u.UserName))
            .RuleFor(user => user.Password, _ => PasswordGenerator.GenerateValid(passwordLength));

        return request;
    }
    
    private static string SanitizeUserName(string userName)
    {
        return MyRegex().Replace(userName, "");
    }

    [GeneratedRegex("[^a-zA-Z0-9_-]")]
    private static partial Regex MyRegex();
}