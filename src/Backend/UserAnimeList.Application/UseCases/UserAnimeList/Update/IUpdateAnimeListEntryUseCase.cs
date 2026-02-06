using UserAnimeList.Communication.Requests;

namespace UserAnimeList.Application.UseCases.UserAnimeList.Update;

public interface IUpdateAnimeListEntryUseCase
{
    public Task Execute(RequestUpdateAnimeListEntryJson request, string id);
}