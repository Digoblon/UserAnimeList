using Microsoft.EntityFrameworkCore;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Infrastructure.Data.Repositories;

public class AnimeRepository : IAnimeRepository
{
    private readonly UserAnimeListDbContext _dbContext;
    
    public AnimeRepository(UserAnimeListDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Anime anime) => await _dbContext.Animes.AddAsync(anime);
    public void Update(Anime anime) => _dbContext.Animes.Update(anime);
    public async Task<double?> AverageScore(Guid animeId)
    {
        var score = await _dbContext
            .AnimeLists
            .Where(l => l.AnimeId == animeId)
            .Where(l => l.IsActive && l.Score != null)
            .AsNoTracking()
            .AverageAsync(l => l.Score);
        
        return score;
    }

    public async Task<bool> ExistsActiveAnimeWithName(string name)
        => await _dbContext.Animes.AnyAsync(anime => anime.NameNormalized == name.ToLower());

    public async Task<Anime?> GetById(string id)
    {
        if(!Guid.TryParse(id, out var animeId))
            throw new InvalidIdException(ResourceMessagesException.INVALID_ID);
        
        return await _dbContext.Animes
            .Include(a => a.Genres)
            .Include(a => a.Studios)
            .FirstOrDefaultAsync(anime => anime.Id == animeId && anime.IsActive && anime.DeletedOn == null);
    }

    public async Task<IList<Anime>> Search(string query)
    {
        if (query.Equals("*"))
            return await _dbContext.Animes.Where(a => a.IsActive && a.DeletedOn == null).ToListAsync();
        
        return await _dbContext
            .Animes
            .AsNoTracking()
            .Where(a => a.IsActive && a.DeletedOn == null)
            .Where(a => a.NameNormalized.Contains(query.ToLower()) || a.Synopsis.ToLower().Contains(query.ToLower()))
            .ToListAsync();
    }
}