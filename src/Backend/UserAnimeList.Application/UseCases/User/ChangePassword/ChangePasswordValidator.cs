using FluentValidation;
using UserAnimeList.Application.SharedValidator;
using UserAnimeList.Communication.Requests;

namespace UserAnimeList.Application.UseCases.User.ChangePassword;

public class ChangePasswordValidator :  AbstractValidator<RequestChangePasswordJson>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.NewPassword).SetValidator(new PasswordValidator<RequestChangePasswordJson>());
    }
}