namespace UserAnimeList.Application.UseCases.Studio.Delete.SoftDelete;

public interface ISoftDeleteStudioUseCase
{
    public Task Execute(string id);
}