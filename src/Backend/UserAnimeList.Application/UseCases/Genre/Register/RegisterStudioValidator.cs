using FluentValidation;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;

namespace UserAnimeList.Application.UseCases.Genre.Register;

public class RegisterGenreValidator : AbstractValidator<RequestRegisterGenreJson>
{
    public RegisterGenreValidator()
    {
        RuleFor(r => r.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
    }
}