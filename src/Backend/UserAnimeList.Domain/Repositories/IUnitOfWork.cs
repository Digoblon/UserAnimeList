namespace UserAnimeList.Domain.Repositories;

public interface IUnitOfWork
{
    public Task Commit();
}