namespace UserAnimeList.Domain.Repositories.User;

public interface IUserRepository
{
    Task Add(Entities.User user);
    public void Update(Entities.User user);
    public void Delete(Entities.User user);
    Task<Entities.User?> GetById(Guid id);
    Task<bool> ExistsActiveUserWithEmail(string email);
    Task<bool> ExistsActiveUserWithUserName(string username);
    Task<Entities.User?> GetByLogin(string login);
}