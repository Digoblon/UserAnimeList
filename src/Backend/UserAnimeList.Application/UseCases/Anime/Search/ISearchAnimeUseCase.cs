using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.Anime.Search;

public interface ISearchAnimeUseCase
{
    public Task<ResponseAnimesJson> Execute(RequestAnimeSearchJson request);
}