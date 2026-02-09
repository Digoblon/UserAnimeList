using UserAnimeList.Communication.Requests;

namespace UserAnimeList.Domain.Repositories.AnimeList;

public interface IAnimeListRepository
{
    Task Add (Entities.AnimeList animeList);
    public void Update (Entities.AnimeList animeList);
    public void Delete (Entities.AnimeList animeList);
    Task<bool> ExistsEntry (Guid userId, Guid animeId);
    Task<Entities.AnimeList?> GetById(string id, Guid userId);
    Task<IList<Entities.AnimeList>> List(Guid userId, RequestAnimeListEntryFilterJson request);
}