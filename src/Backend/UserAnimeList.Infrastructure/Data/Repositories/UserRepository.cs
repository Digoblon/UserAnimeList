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
    
    public async Task AddAsync(User user) => await _dbContext.Users.AddAsync(user);
    

    public async Task<bool> ExistsActiveUserWithEmailAsync(string email)
        => await _dbContext.Users.AnyAsync(u=>u.Email.Equals(email));
    
    public async Task<bool> ExistsActiveUserWithUserNameAsync(string username)
        => await _dbContext.Users.AnyAsync(u=>u.UserName.Equals(username));
    
}