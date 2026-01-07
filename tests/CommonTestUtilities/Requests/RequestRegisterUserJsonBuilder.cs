using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestRegisterUserJsonBuilder
{
    public static RequestRegisterUserJson Build(int passwordLength = 10)
    {
        return new Faker<RequestRegisterUserJson>()
            .RuleFor(user => user.UserName, (f) => f.Internet.UserName())
            .RuleFor(user => user.Email, (f, u) => f.Internet.Email(u.UserName))
            .RuleFor(user => user.Password, _ => PasswordGenerator.GenerateValid(passwordLength));
    }
}