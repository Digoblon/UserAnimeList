namespace UserAnimeList.Application.UseCases.Anime.Delete.SoftDelete;

public interface ISoftDeleteAnimeUseCase
{
    public Task Execute(string id);
}