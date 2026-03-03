namespace UserAnimeList.Application.UseCases.Genre.Delete.SoftDelete;

public interface ISoftDeleteGenreUseCase
{
    public Task Execute(string id);
}