using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.Anime.Register;

public interface IRegisterAnimeUseCase
{
    public Task<ResponseRegisteredAnimeJson> Execute(RequestAnimeJson request);
}