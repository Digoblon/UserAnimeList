using FluentValidation;
using UserAnimeList.Application.SharedValidator;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;

namespace UserAnimeList.Application.UseCases.User.Register;

public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
{
    public RegisterUserValidator()
    {
        RuleFor(r => r.UserName).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(user => user.Email).NotEmpty().WithMessage(ResourceMessagesException.EMAIL_EMPTY);
        RuleFor(r => r.Password).SetValidator(new PasswordValidator<RequestRegisterUserJson>());
        When(user => !string.IsNullOrEmpty(user.Email), () =>
            {
                RuleFor(user => user.Email).EmailAddress().WithMessage(ResourceMessagesException.EMAIL_INVALID);
            }
        );
            
    }
}