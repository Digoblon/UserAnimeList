using Microsoft.EntityFrameworkCore;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.Studio;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Infrastructure.Data.Repositories;

public class StudioRepository : IStudioRepository
{
    private readonly UserAnimeListDbContext _dbContext;

    public StudioRepository(UserAnimeListDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Studio studio) => await _dbContext.Studios.AddAsync(studio);
    public void Update(Studio studio) => _dbContext.Studios.Update(studio);


    public async Task<bool> ExistsActiveStudioWithName(string name)
        => await _dbContext.Studios.AnyAsync(u=>u.NameNormalized.Equals(name.ToLower()));

    public async Task<Studio?> GetById(string id)
    {
         if(!Guid.TryParse(id, out var studioId))
            throw new InvalidIdException(ResourceMessagesException.INVALID_ID);
         
         return await _dbContext.Studios.AsNoTracking().FirstOrDefaultAsync(s => s.Id == studioId);
    }

    public async Task<Studio?> GetByName(string name) 
        => await _dbContext.Studios.AsNoTracking().FirstOrDefaultAsync(s => s.IsActive && s.NameNormalized == name.ToLower());

    public async Task<IList<Studio>> SearchByName(string name)
    {
        return await _dbContext.Studios.AsNoTracking().Where(s => s.IsActive && s.NameNormalized.Contains(name.ToLower())).ToListAsync();
    }

    public async Task<IList<Guid>> GetStudiosInList(IList<Guid> studios)
    {
        if (studios is null || studios.Count == 0)
            return [];
        
        return await _dbContext.Studios
            .Where(s => studios.Contains(s.Id))
            .Where(s => s.DeletedOn == null && s.IsActive)
            .Select(s => s.Id)
            .ToListAsync();
    }
}