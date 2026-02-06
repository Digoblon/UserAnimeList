namespace UserAnimeList.Application.UseCases.UserAnimeList.Delete;

public interface IDeleteAnimeListEntryUseCase
{
    public Task Execute(string id);
}