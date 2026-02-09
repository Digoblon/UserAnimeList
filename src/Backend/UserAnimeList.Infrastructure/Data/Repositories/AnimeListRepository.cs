using Microsoft.EntityFrameworkCore;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Repositories.AnimeList;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;
using AnimeEntryStatus = UserAnimeList.Domain.Enums.AnimeEntryStatus;

namespace UserAnimeList.Infrastructure.Data.Repositories;

public class AnimeListRepository : IAnimeListRepository
{
    private readonly UserAnimeListDbContext _dbContext;
    
    public AnimeListRepository(UserAnimeListDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Add(Domain.Entities.AnimeList animeList) => await _dbContext.AnimeLists.AddAsync(animeList);
    public void Update(Domain.Entities.AnimeList animeList) => _dbContext.AnimeLists.Update(animeList);
    public void Delete(Domain.Entities.AnimeList animeList) =>  _dbContext.AnimeLists.Remove(animeList);
    

    public async Task<bool> ExistsEntry(Guid userId, Guid animeId)
        => await _dbContext.AnimeLists.AnyAsync(l => l.UserId == userId && l.AnimeId == animeId);

    public async Task<Domain.Entities.AnimeList?> GetById(string id, Guid userId)
    {
        if(!Guid.TryParse(id, out var animeListId))
            throw new InvalidIdException(ResourceMessagesException.INVALID_ID);

        return await _dbContext.AnimeLists
            .AsNoTracking()
            .Include(l => l.User)
            .Include(l => l.Anime)
            .FirstOrDefaultAsync(l =>  l.IsActive && l.UserId == userId && (l.Id == animeListId || l.AnimeId == animeListId));
    }

    public async Task<IList<Domain.Entities.AnimeList>> List(Guid userId, RequestAnimeListEntryFilterJson filter)
    {
        var query = _dbContext.AnimeLists
            .AsNoTracking()
            .Include(l => l.Anime)
            .Where(l => l.UserId == userId);
        
        if (filter.Status.HasValue)
            query = query.Where(l => l.Status == (AnimeEntryStatus)filter.Status.Value);

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            query = query.Where(l => l.Anime.NameNormalized.Contains(filter.Query.ToLower()));
        }
        
        if (filter.DateStarted.HasValue)
            query = query.Where(l => l.DateStarted.HasValue && l.DateStarted.Value >= filter.DateStarted.Value);
        
        if (filter.DateFinished.HasValue)
            query = query.Where(l => l.DateFinished.HasValue && l.DateFinished.Value <= filter.DateFinished.Value);
        
        var field = filter.SortField ?? AnimeListSort.Name;
        var desc = filter.SortDirection == SortDirection.Desc;

        query = field switch
        {
            AnimeListSort.Name => desc
                ? query.OrderByDescending(l => l.Anime.NameNormalized)
                : query.OrderBy(l => l.Anime.NameNormalized),

            AnimeListSort.Score => desc
                ? query.OrderByDescending(l => l.Score)
                : query.OrderBy(l => l.Score),

            AnimeListSort.Progress => desc
                ? query.OrderByDescending(l => l.Progress)
                : query.OrderBy(l => l.Progress),

            AnimeListSort.Status => desc
                ? query.OrderByDescending(l => l.Status)
                : query.OrderBy(l => l.Status),
            
            AnimeListSort.Type => desc
                ? query.OrderByDescending(l => l.Anime.Type)
                : query.OrderBy(l => l.Anime.Type),

            AnimeListSort.DateStarted => desc
                ? query.OrderByDescending(l => l.DateStarted)
                : query.OrderBy(l => l.DateStarted),

            AnimeListSort.DateFinished => desc
                ? query.OrderByDescending(l => l.DateFinished)
                : query.OrderBy(l => l.DateFinished),

            _ => query.OrderBy(l => l.Anime.NameNormalized)
        };

        
        return await query.ToListAsync();
    }
}