using FluentValidation;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;

namespace UserAnimeList.Application.UseCases.AnimeList.List;

public class AnimeListFilterValidator : AbstractValidator<RequestAnimeListEntryFilterJson>
{
    public AnimeListFilterValidator()
    {
        RuleFor(x => x.Status).IsInEnum().WithMessage(ResourceMessagesException.ANIME_LIST_INVALID_STATUS);
        RuleFor(x => x.Query).Cascade(CascadeMode.Stop).MaximumLength(256).WithMessage(ResourceMessagesException.ANIME_LIST_QUERY_EXCEEDED);
        RuleFor(x => x.SortField).IsInEnum().WithMessage(ResourceMessagesException.ANIME_LIST_INVALID_SORT_FIELD);
        RuleFor(x => x.SortDirection).IsInEnum().WithMessage(ResourceMessagesException.ANIME_LIST_INVALID_SORT_DIR);
        
    }
}