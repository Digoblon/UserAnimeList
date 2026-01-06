using UserAnimeList.Domain.Repositories;

namespace UserAnimeList.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly UserAnimeListDbContext _dbContext;

    public UnitOfWork(UserAnimeListDbContext dbContext) => _dbContext = dbContext;

    public async Task Commit() => await _dbContext.SaveChangesAsync();
}