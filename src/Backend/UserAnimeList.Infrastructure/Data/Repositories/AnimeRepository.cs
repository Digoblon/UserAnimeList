using Microsoft.EntityFrameworkCore;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;
using AnimeStatus = UserAnimeList.Domain.Enums.AnimeStatus;
using AnimeType = UserAnimeList.Domain.Enums.AnimeType;
using Season = UserAnimeList.Domain.Enums.Season;

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

    public async Task<IList<Anime>> Filter(RequestAnimeFilterJson filter)
    {
        var query = _dbContext.Animes
            .AsNoTracking()
            .Where(a => a.DeletedOn == null && a.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            var q = filter.Query.ToLower();
            query = query.Where(a => a.NameNormalized.Contains(q));
        }

        if (filter.Status.HasValue)
            query = query.Where(a => a.Status == (AnimeStatus)filter.Status.Value);

        if (filter.Type.HasValue)
            query = query.Where(a => a.Type == (AnimeType)filter.Type.Value);

        if (filter.AiredFrom.HasValue)
            query = query.Where(a => a.AiredFrom.HasValue && a.AiredFrom.Value >= filter.AiredFrom.Value);

        if (filter.AiredUntil.HasValue)
            query = query.Where(a => a.AiredUntil.HasValue && a.AiredUntil.Value <= filter.AiredUntil.Value);

        if (filter.Genres is { Count: > 0 })
            query = query.Where(a => a.Genres.Any(ag => filter.Genres.Contains(ag.GenreId)));

        if (filter.Studios is { Count: > 0 })
            query = query.Where(a => a.Studios.Any(ast => filter.Studios.Contains(ast.StudioId)));

        if (filter.PremieredSeason.HasValue && filter.PremieredYear.HasValue)
        {
            var (start, end) = GetSeasonRange((Season)filter.PremieredSeason.Value, filter.PremieredYear.Value);

            query = query.Where(a => a.AiredFrom.HasValue
                                     && a.AiredFrom.Value >= start
                                     && a.AiredFrom.Value <= end);
        }

        var field = filter.SortField ?? AnimeSort.Name;
        var desc = filter.SortDirection == SortDirection.Desc;

        query = field switch
        {
            AnimeSort.Name => desc
                ? query.OrderByDescending(a => a.NameNormalized)
                : query.OrderBy(a => a.NameNormalized),

            AnimeSort.Episodes => desc
                ? query.OrderByDescending(a => a.Episodes)
                : query.OrderBy(a => a.Episodes),

            AnimeSort.Status => desc
                ? query.OrderByDescending(a => a.Status)
                : query.OrderBy(a => a.Status),

            AnimeSort.Type => desc
                ? query.OrderByDescending(a => a.Type)
                : query.OrderBy(a => a.Type),

            AnimeSort.AiredFrom => desc
                ? query.OrderByDescending(a => a.AiredFrom)
                : query.OrderBy(a => a.AiredFrom),

            AnimeSort.AiredUntil => desc
                ? query.OrderByDescending(a => a.AiredUntil)
                : query.OrderBy(a => a.AiredUntil),

            AnimeSort.Premiered => desc
                ? query.OrderByDescending(a => a.AiredFrom)
                : query.OrderBy(a => a.AiredFrom),

            _ => query.OrderBy(a => a.NameNormalized)
        };

        return await query.ToListAsync();
    }

    private static (DateOnly start, DateOnly end) GetSeasonRange(Season season, int year)
    {
        return season switch
        {
            Season.Winter => (new DateOnly(year, 1, 1),  new DateOnly(year, 3, 31)),
            Season.Spring => (new DateOnly(year, 4, 1),  new DateOnly(year, 6, 30)),
            Season.Summer => (new DateOnly(year, 7, 1),  new DateOnly(year, 9, 30)),
            Season.Fall   => (new DateOnly(year, 10, 1), new DateOnly(year, 12, 31)),
            _ => throw new ErrorOnValidationException([ResourceMessagesException.INVALID_SEASON])
        };
    }
}
