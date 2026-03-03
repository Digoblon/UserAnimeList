using FluentValidation;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;

namespace UserAnimeList.Application.UseCases.Studio.Update;

public class UpdateStudioValidator : AbstractValidator<RequestUpdateStudioJson>
{
    public UpdateStudioValidator()
    {
        RuleFor(r => r.Name).NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY);
    }
}