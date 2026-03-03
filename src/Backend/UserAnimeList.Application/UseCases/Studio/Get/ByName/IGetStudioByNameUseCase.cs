using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.Studio.Get.ByName;

public interface IGetStudioByNameUseCase
{
    public Task<ResponseStudiosJson> Execute (RequestStudioGetByNameJson  request);
}