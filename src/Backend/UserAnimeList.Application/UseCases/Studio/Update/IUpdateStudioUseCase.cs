using UserAnimeList.Communication.Requests;

namespace UserAnimeList.Application.UseCases.Studio.Update;

public interface IUpdateStudioUseCase
{
    public Task Execute(RequestUpdateStudioJson request, string id);
}