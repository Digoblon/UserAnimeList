namespace UserAnimeList.Application.UseCases.AnimeList.Delete;

public interface IDeleteAnimeListEntryUseCase
{
    public Task Execute(string id);
}