using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.AnimeList.Get.ById;

public interface IGetAnimeListEntryByIdUseCase
{
    Task<ResponseAnimeListEntryJson> Execute(string id);
}