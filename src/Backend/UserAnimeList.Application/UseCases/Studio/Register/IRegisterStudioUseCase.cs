using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.Studio.Register;

public interface IRegisterStudioUseCase
{
    public Task<ResponseRegisteredStudioJson> Execute(RequestRegisterStudioJson request);
}