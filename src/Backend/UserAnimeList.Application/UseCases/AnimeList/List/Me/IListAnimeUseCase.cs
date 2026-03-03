using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.AnimeList.List.Me;

public interface IListAnimeUseCase
{
    Task<ResponseAnimeListsJson> Execute(RequestAnimeListEntryFilterJson request);
}