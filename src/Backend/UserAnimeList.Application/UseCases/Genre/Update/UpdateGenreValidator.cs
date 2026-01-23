using FluentValidation;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;

namespace UserAnimeList.Application.UseCases.Genre.Update;

public class UpdateGenreValidator : AbstractValidator<RequestUpdateGenreJson>
{
    public UpdateGenreValidator()
    {
        RuleFor(r => r.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
    }
}