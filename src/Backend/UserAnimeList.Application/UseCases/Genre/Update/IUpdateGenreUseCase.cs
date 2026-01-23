using UserAnimeList.Communication.Requests;

namespace UserAnimeList.Application.UseCases.Genre.Update;

public interface IUpdateGenreUseCase
{
    public Task Execute(RequestUpdateGenreJson request, string id);
}