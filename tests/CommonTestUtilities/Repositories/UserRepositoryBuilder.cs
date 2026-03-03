using Moq;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories;

public class UserRepositoryBuilder
{
    private readonly Mock<IUserRepository> _repository;

    public UserRepositoryBuilder() =>  _repository = new Mock<IUserRepository>();

    public void ExistActiveUserWithEmail(string email)
    {
        _repository.Setup(repository => repository.ExistsActiveUserWithEmail(email)).ReturnsAsync(true);
    }
    
    public UserRepositoryBuilder GetById(User user)
    {
        _repository.Setup(x=> x.GetById(user.Id)).ReturnsAsync(user);
        return this;
    }
    
    public void GetByLogin(User user, string login)
    {
        
        _repository.Setup(repository => repository.GetByLogin(login)).ReturnsAsync(user);
    }
    
    public IUserRepository Build() =>  _repository.Object;
}