using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.UserAnimeList.List;

public interface IListAnimeByUserIdUseCase
{
    Task<ResponseAnimeListsJson> Execute(string userId, RequestAnimeListEntryFilterJson request);
}