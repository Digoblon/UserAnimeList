namespace UserAnimeList.Domain.Repositories.Studio;

public interface IStudioRepository
{
    Task Add(Entities.Studio studio);
    public void Update(Entities.Studio studio);
    Task<bool> ExistsActiveStudioWithName(string name);
    Task<Entities.Studio?> GetById (string id);
    Task<Entities.Studio?> GetByName (string name);
    Task<IList<Entities.Studio>> SearchByName (string name);
    Task<IList<Guid>> GetStudiosInList (IList<Guid> studios);
}