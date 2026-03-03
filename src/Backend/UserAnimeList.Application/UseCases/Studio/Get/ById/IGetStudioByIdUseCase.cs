using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.Studio.Get.ById;

public interface IGetStudioByIdUseCase
{
    public Task<ResponseStudioJson>  Execute(string id);
}