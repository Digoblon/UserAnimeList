namespace UserAnimeList.Domain.Repositories.User;

public interface IUserRepository
{
    Task AddAsync(Entities.User user);
    //Task<Entities.User?> GetByIdAsync(Guid id);
    //Task<Entities.User?> GetByEmailAsync(string email);
    Task<bool> ExistsActiveUserWithEmailAsync(string email);
    Task<bool> ExistsActiveUserWithUserNameAsync(string username);
}