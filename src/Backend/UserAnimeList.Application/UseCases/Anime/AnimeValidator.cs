using FluentValidation;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;

namespace UserAnimeList.Application.UseCases.Anime;

public class AnimeValidator : AbstractValidator<RequestAnimeJson>
{
    public AnimeValidator()
    {
        RuleFor(a => a.Name)
            .NotEmpty().WithMessage(ResourceMessagesException.NAME_EMPTY)
            .MaximumLength(256).WithMessage(ResourceMessagesException.ANIME_EXCEEDS_LIMIT);

        When(a => !string.IsNullOrWhiteSpace(a.Synopsis), () =>
        {
            RuleFor(a => a.Synopsis)
                .MaximumLength(5000).WithMessage(ResourceMessagesException.SYNOPSIS_EXCEEDS_LIMIT);
        });

        When(a => a.Episodes.HasValue, () =>
        {
            RuleFor(a => a.Episodes!.Value)
                .GreaterThan(0).WithMessage(ResourceMessagesException.EPISODE_COUNT_INVALID);
        });

        RuleFor(a => a.Status)
            .IsInEnum().WithMessage(ResourceMessagesException.STATUS_NOT_SUPPORTED);

        RuleFor(a => a.Source)
            .IsInEnum().WithMessage(ResourceMessagesException.SOURCE_NOT_SUPPORTED);

        RuleFor(a => a.Type)
            .IsInEnum().WithMessage(ResourceMessagesException.TYPE_NOT_SUPPORTED);

        RuleFor(a => a.Genres)
            .NotNull().WithMessage(ResourceMessagesException.GENRES_CANNOT_BE_NULL);

        RuleFor(a => a.Studios)
            .NotNull().WithMessage(ResourceMessagesException.STUDIOS_CANNOT_BE_NULL);

        RuleFor(a => a.Genres)
            .Must(g => g.Distinct().Count() == g.Count)
            .WithMessage(ResourceMessagesException.DUPLICATED_GENRES);

        RuleFor(a => a.Studios)
            .Must(s => s.Distinct().Count() == s.Count)
            .WithMessage(ResourceMessagesException.DUPLICATED_STUDIOS);

        RuleFor(x => x.AiredFrom)
            .NotNull()
            .When(x => x.AiredUntil.HasValue)
            .WithMessage(ResourceMessagesException.ANIME_AIRED_UNTIL_FILLED_AIRED_FROM_NOT_FILLED);

        RuleFor(x => x)
            .Must(x =>
                !x.AiredFrom.HasValue ||
                !x.AiredUntil.HasValue ||
                x.AiredUntil.Value >= x.AiredFrom.Value
            )
            .WithMessage(ResourceMessagesException.ANIME_AIRED_UNTIL_EARLY_FROM);

        When(a => a.Status == AnimeStatus.Finished, () =>
        {
            RuleFor(a => a.AiredFrom)
                .NotNull().WithMessage(ResourceMessagesException.ANIME_AIRED_FROM_REQUIRED_WHEN_FINISHED);
        });

        When(a => a.Status == AnimeStatus.Finished, () =>
        {
            RuleFor(a => a.AiredUntil)
                .NotNull().WithMessage(ResourceMessagesException.ANIME_AIRED_UNTIL_REQUIRED_WHEN_FINISHED);
        });

        When(a => a.Status == AnimeStatus.Finished, () =>
        {
            RuleFor(a => a.Episodes)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(ResourceMessagesException.EPISODE_MUST_BE_SPECIFIED);
        });

        When(a => a.Status == AnimeStatus.Airing, () =>
        {
            RuleFor(a => a.AiredFrom)
                .NotNull().WithMessage(ResourceMessagesException.ANIME_AIRED_FROM_REQUIRED_WHEN_AIRING);
        });
    }
}