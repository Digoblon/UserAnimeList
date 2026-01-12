using FluentValidation;
using UserAnimeList.Application.SharedValidator;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;

namespace UserAnimeList.Application.UseCases.User.Update;

public class UpdateUserValidator: AbstractValidator<RequestUpdateUserJson>
{
    public UpdateUserValidator() 
    {
        RuleFor(request => request.UserName).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
        RuleFor(request => request.Email).NotEmpty().WithMessage(ResourceMessagesException.EMAIL_EMPTY);
        When(user => !string.IsNullOrEmpty(user.UserName), () =>
            {
                RuleFor(user => user.UserName).SetValidator(new UserNameValidator<RequestUpdateUserJson>());
            }
        );
        When(request => !string.IsNullOrWhiteSpace(request.Email), () =>
        {
            RuleFor(request => request.Email).EmailAddress().WithMessage(ResourceMessagesException.EMAIL_INVALID);
        });
    }
}