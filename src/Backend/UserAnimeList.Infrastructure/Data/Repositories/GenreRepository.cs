using Microsoft.EntityFrameworkCore;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.Genre;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Infrastructure.Data.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly UserAnimeListDbContext _dbContext;

    public GenreRepository(UserAnimeListDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Genre genre) => await _dbContext.Genres.AddAsync(genre);
    public void Update(Genre genre) => _dbContext.Genres.Update(genre);


    public async Task<bool> ExistsActiveGenreWithName(string name)
        => await _dbContext.Genres.AnyAsync(u=>u.NameNormalized.Equals(name.ToLower()));

    public async Task<Genre?> GetById(string id)
    {
        if(!Guid.TryParse(id, out var genreId))
            throw new InvalidIdException(ResourceMessagesException.INVALID_ID);
         
        return await _dbContext.Genres.AsNoTracking().FirstOrDefaultAsync(s => s.Id == genreId);
    }

    public async Task<Genre?> GetByName(string name) 
        => await _dbContext.Genres.AsNoTracking().FirstOrDefaultAsync(s => s.IsActive && s.NameNormalized == name.ToLower());

    public async Task<IList<Genre>> SearchByName(string name)
    {
        return await _dbContext.Genres.AsNoTracking().Where(s => s.IsActive && s.NameNormalized.Contains(name.ToLower())).ToListAsync();
    }

    public async Task<IList<Guid>> GetGenresInList(IList<Guid> genres)
    {
        if (genres is null || genres.Count == 0)
            return [];
        
        return await _dbContext.Genres
            .Where(g => genres.Contains(g.Id))
            .Where(g => g.DeletedOn == null && g.IsActive)
            .Select(g => g.Id)
            .ToListAsync();
    }
}