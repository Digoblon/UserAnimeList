namespace UserAnimeList.Domain.Repositories.User;

public interface IUserRepository
{
    Task AddAsync(Entities.User user);
    //Task<Entities.User?> GetByIdAsync(Guid id);
    //Task<Entities.User?> GetByEmailAsync(string email);
    Task<bool> ExistsActiveUserWithEmail(string email);
    Task<bool> ExistsActiveUserWithUserName(string username);
}