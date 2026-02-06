using FluentValidation;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;

namespace UserAnimeList.Application.UseCases.AnimeList.Update;

public class UpdateAnimeListEntryValidator : AbstractValidator<RequestUpdateAnimeListEntryJson>
{
    public UpdateAnimeListEntryValidator()
    {
        RuleFor(l => l.Status).IsInEnum().WithMessage(ResourceMessagesException.ANIME_LIST_INVALID_STATUS);
        When(l => l.Score.HasValue, () =>
        {
            RuleFor(l => l.Score)
                .Cascade(CascadeMode.Stop)
                .InclusiveBetween(1,10).WithMessage(ResourceMessagesException.ANIME_LIST_SCORE_INVALID);
        });
        When(l => l.Progress.HasValue, () =>
        {
            RuleFor(l => l.Progress)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0).WithMessage(ResourceMessagesException.EPISODE_COUNT_INVALID);
        });

        RuleFor(l => l)
            .Must(l =>
                !l.DateStarted.HasValue ||
                !l.DateFinished.HasValue ||
                l.DateFinished.Value >= l.DateStarted.Value
            ).WithMessage(ResourceMessagesException.ANIME_LIST_DATE_FINISHED_EARLIER_DATE_STARTED);
        
        
    }
}