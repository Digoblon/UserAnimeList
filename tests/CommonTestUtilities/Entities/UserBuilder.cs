using Bogus;
using CommonTestUtilities.Cryptography;
using UserAnimeList.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class UserBuilder
{
    public static (User user,string password) Build()
    {
        var passwordEncrypter = PasswordEncrypterBuilder.Build();

        var password = new Faker().Internet.Password();
        var user =  new Faker<User>()
            .RuleFor(user => user.Id, Guid.NewGuid)
            .RuleFor(user => user.UserName, (f) => f.Internet.UserName())
            .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.UserName))
            .RuleFor(user => user.Password, _ => passwordEncrypter.Encrypt(password));

        return (user, password);

    }
}