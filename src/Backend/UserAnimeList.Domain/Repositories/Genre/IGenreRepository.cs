namespace UserAnimeList.Domain.Repositories.Genre;

public interface IGenreRepository
{
    Task Add(Entities.Genre genre);
    public void Update(Entities.Genre genre);
    Task<bool> ExistsActiveGenreWithName(string name);
    Task<Entities.Genre?> GetById (string id);
    Task<Entities.Genre?> GetByName (string name);
    Task<IList<Entities.Genre>> SearchByName (string name);
}