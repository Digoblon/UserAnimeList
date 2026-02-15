using Microsoft.EntityFrameworkCore;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.User;

namespace UserAnimeList.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserAnimeListDbContext _dbContext;

    public UserRepository(UserAnimeListDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task Add(User user) => await _dbContext.Users.AddAsync(user);
    public void Update(User user) => _dbContext.Users.Update(user);
    public void Delete(User user)
    {
        user.DeletedOn = DateTime.UtcNow;
        user.IsActive = false;
        user.IncrementTokenVersion();
        
        Update(user);
    }

    public async Task<User?> GetById(Guid id)
    {
        return await _dbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }


    public async Task<bool> ExistsActiveUserWithEmail(string email)
        => await _dbContext.Users.AnyAsync(u=>u.Email.Equals(email));
    
    public async Task<bool> ExistsActiveUserWithUserName(string username)
        => await _dbContext.Users.AnyAsync(u=>u.UserName.Equals(username));

    public async Task<bool> ExistActiveUserWithId(Guid id)
        => await _dbContext.Users.AnyAsync(u => u.Id.Equals(id) && u.IsActive);

    public async Task<User?> GetByLogin(string login)
    {
        login = login.Trim().ToLower();

        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                u.IsActive &&
                (u.Email.ToLower() == login || u.UserName.ToLower() == login));
    }
}