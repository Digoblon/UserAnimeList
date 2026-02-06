using UserAnimeList.Communication.Requests;

namespace UserAnimeList.Domain.Repositories.UserAnimeList;

public interface IUserAnimeListRepository
{
    Task Add (Entities.UserAnimeList animeList);
    public void Update (Entities.UserAnimeList animeList);
    public void Delete (Entities.UserAnimeList animeList);
    Task<bool> ExistsEntry (Guid userId, Guid animeId);
    Task<Entities.UserAnimeList?> GetById(string id, Guid userId);
    Task<IList<Entities.UserAnimeList>> List(Guid userId, RequestAnimeListEntryFilterJson request);
}