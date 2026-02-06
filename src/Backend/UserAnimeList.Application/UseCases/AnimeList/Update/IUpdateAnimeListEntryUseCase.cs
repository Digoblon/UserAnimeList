using UserAnimeList.Communication.Requests;

namespace UserAnimeList.Application.UseCases.AnimeList.Update;

public interface IUpdateAnimeListEntryUseCase
{
    public Task Execute(RequestUpdateAnimeListEntryJson request, string id);
}