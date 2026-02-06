using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.UserAnimeList.Get.ById;

public interface IGetAnimeListEntryByIdUseCase
{
    Task<ResponseAnimeListEntryJson> Execute(string id);
}