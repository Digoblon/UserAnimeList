using UserAnimeList.Communication.Requests;

namespace UserAnimeList.Application.UseCases.Anime.Update;

public interface IUpdateAnimeUseCase
{
    public Task Execute(RequestAnimeJson request, string id);
}