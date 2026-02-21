using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Domain.Repositories.Anime;

public interface IAnimeRepository
{
    Task Add (Entities.Anime anime);
    public void Update (Entities.Anime anime);
    Task<double?> AverageScore (Guid animeId);
    Task<bool> ExistsActiveAnimeWithName(string name);
    Task<Entities.Anime?> GetById (string id);
    Task<IList<Entities.Anime>> Search(string query);
    Task<IList<ResponseShortAnimeJson>> SearchWithScore(string query);
    Task<IList<Entities.Anime>> Filter(RequestAnimeFilterJson filter);
    Task<IList<ResponseShortAnimeJson>> FilterWithScore(RequestAnimeFilterJson filter);
}