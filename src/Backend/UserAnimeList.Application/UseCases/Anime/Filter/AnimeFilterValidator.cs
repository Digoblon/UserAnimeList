using FluentValidation;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;

namespace UserAnimeList.Application.UseCases.Anime.Filter;

public class AnimeFilterValidator : AbstractValidator<RequestAnimeFilterJson>
{
    public AnimeFilterValidator()
    {
        RuleFor(x => x.Query)
            .MaximumLength(256)
            .WithMessage(ResourceMessagesException.ANIME_QUERY_EXCEEDED);

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(ResourceMessagesException.STATUS_NOT_SUPPORTED);

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage(ResourceMessagesException.TYPE_NOT_SUPPORTED);

        RuleFor(x => x.SortField)
            .IsInEnum()
            .WithMessage(ResourceMessagesException.ANIME_INVALID_SORT_FIELD);

        RuleFor(x => x.SortDirection)
            .IsInEnum()
            .WithMessage(ResourceMessagesException.ANIME_INVALID_SORT_DIR);

        RuleFor(x => x.PremieredYear)
            .NotNull()
            .When(x => x.PremieredSeason.HasValue)
            .WithMessage(ResourceMessagesException.ANIME_PREMIERED_YEAR_REQUIRED);

        RuleFor(x => x.PremieredSeason)
            .IsInEnum()
            .WithMessage(ResourceMessagesException.INVALID_SEASON);

        RuleFor(x => x.PremieredSeason)
            .NotNull()
            .When(x => x.PremieredYear.HasValue)
            .WithMessage(ResourceMessagesException.ANIME_PREMIERED_SEASON_REQUIRED);

        RuleFor(x => x)
            .Must(x =>
                !x.AiredFrom.HasValue ||
                !x.AiredUntil.HasValue ||
                x.AiredUntil.Value >= x.AiredFrom.Value)
            .WithMessage(ResourceMessagesException.ANIME_AIRED_UNTIL_EARLY_FROM);

        RuleFor(x => x.Genres)
            .Must(list => list is null || list.Distinct().Count() == list.Count)
            .WithMessage(ResourceMessagesException.DUPLICATED_GENRES);

        RuleFor(x => x.Studios)
            .Must(list => list is null || list.Distinct().Count() == list.Count)
            .WithMessage(ResourceMessagesException.DUPLICATED_STUDIOS);
    }
}
