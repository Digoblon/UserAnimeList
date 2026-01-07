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
    
    /*
    public void GetByEmail(User user)
    {
        _repository.Setup(repository => repository.GetB(user.Email)).ReturnsAsync(user);
    }*/
    
    public IUserRepository Build() =>  _repository.Object;
}