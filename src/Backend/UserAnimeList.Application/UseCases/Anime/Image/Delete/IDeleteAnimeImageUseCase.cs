namespace UserAnimeList.Application.UseCases.Anime.Image.Delete;

public interface IDeleteAnimeImageUseCase
{
    Task Execute(string id);
}