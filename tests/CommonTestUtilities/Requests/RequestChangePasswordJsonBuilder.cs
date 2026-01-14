using Bogus;
using UserAnimeList.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestChangePasswordJsonBuilder
{
    public static RequestChangePasswordJson Build(int passwordLength = 10)
    {
        return new Faker<RequestChangePasswordJson>()
            .RuleFor(user => user.NewPassword, (f) => f.Internet.Password(passwordLength))
            .RuleFor(user => user.Password, _ => PasswordGenerator.GenerateValid(passwordLength))
            .RuleFor(user => user.ConfirmNewPassword, (_, u) => u.NewPassword);

    }
}