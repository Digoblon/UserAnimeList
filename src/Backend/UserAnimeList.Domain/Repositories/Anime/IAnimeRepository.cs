namespace UserAnimeList.Domain.Repositories.Anime;

public interface IAnimeRepository
{
    Task Add (Entities.Anime anime);
    public void Update (Entities.Anime anime);
    Task<double?> AverageScore (Guid animeId);
    Task<bool> ExistsActiveAnimeWithName(string name);
    Task<Entities.Anime?> GetById (string id);
    Task<IList<Entities.Anime>> Search(string query);
}