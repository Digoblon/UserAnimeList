using System.Text.RegularExpressions;
using Bogus;
using CommonTestUtilities.Cryptography;
using UserAnimeList.Domain.Entities;

namespace CommonTestUtilities.Entities;

public partial class UserBuilder
{
    public static (User user,string password) Build()
    {
        var passwordEncrypter = PasswordEncrypterBuilder.Build();

        var password = new Faker().Internet.Password();
        var user =  new Faker<User>()
            .RuleFor(user => user.Id, Guid.NewGuid)
            .RuleFor(user => user.UserName, (f) => SanitizeUserName(f.Internet.UserName()))
            .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.UserName))
            .RuleFor(user => user.Password, _ => passwordEncrypter.Encrypt(password));
        return (user, password);

    }
    
    private static string SanitizeUserName(string userName)
    {
        return MyRegex().Replace(userName, "");
    }

    [GeneratedRegex("[^a-zA-Z0-9_-]")]
    private static partial Regex MyRegex();
}
