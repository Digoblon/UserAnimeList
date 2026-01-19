namespace UserAnimeList.Domain.Repositories.User;

public interface IUserRepository
{
    Task Add(Entities.User user);
    public void Update(Entities.User user);
    Task<Entities.User?> GetById(Guid id);
    //Task<Entities.User?> GetByEmail(string email);
    Task<bool> ExistsActiveUserWithEmail(string email);
    Task<bool> ExistsActiveUserWithUserName(string username);
    public Task<bool> ExistActiveUserWithId(Guid userIdentifier);
    Task<Entities.User?> GetByLogin(string login);
}