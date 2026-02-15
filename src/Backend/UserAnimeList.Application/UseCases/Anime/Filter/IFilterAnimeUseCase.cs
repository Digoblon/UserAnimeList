using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.Anime.Filter;

public interface IFilterAnimeUseCase
{
    Task<ResponseAnimesJson> Execute(RequestAnimeFilterJson request);
}