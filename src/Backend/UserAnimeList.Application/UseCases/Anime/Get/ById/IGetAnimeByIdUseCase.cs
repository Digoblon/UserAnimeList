using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.Anime.Get.ById;

public interface IGetAnimeByIdUseCase
{
    public Task<ResponseAnimeJson> Execute(string id);
}