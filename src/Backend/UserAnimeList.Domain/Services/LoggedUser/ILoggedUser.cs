using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Domain.Services.LoggedUser;

public interface ILoggedUser
{
    public Task<User> User();
}