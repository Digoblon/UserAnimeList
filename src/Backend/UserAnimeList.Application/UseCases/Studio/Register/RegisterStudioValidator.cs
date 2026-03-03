using FluentValidation;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;

namespace UserAnimeList.Application.UseCases.Studio.Register;

public class RegisterStudioValidator : AbstractValidator<RequestRegisterStudioJson>
{
    public RegisterStudioValidator()
    {
        RuleFor(r => r.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
    }
}