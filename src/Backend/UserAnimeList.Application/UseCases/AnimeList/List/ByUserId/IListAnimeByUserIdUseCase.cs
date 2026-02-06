using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.AnimeList.List.ByUserId;

public interface IListAnimeByUserIdUseCase
{
    Task<ResponseAnimeListsJson> Execute(string userId, RequestAnimeListEntryFilterJson request);
}