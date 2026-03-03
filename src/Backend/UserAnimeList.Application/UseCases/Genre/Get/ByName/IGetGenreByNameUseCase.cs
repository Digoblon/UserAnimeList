using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.Genre.Get.ByName;

public interface IGetGenreByNameUseCase
{
    public Task<ResponseGenresJson> Execute (RequestGenreGetByNameJson  request);
}