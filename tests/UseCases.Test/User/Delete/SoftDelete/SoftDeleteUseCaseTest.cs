using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using UserAnimeList.Application.UseCases.User.Delete.SoftDelete;

namespace UseCases.Test.User.Delete.SoftDelete;

public class SoftDeleteUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        (var user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);
        
        Func<Task> act = async () => await useCase.Execute();

        await act();
    }
    
    [Fact]
    public async Task Error_User_Not_Active()
    {
        (var user, _) = UserBuilder.Build();
        user.IsActive = false;

        var useCase = CreateUseCase(user);
        
        Func<Task> act = async () => await useCase.Execute();

        await act();
    }
    
    
    private static SoftDeleteUseCase CreateUseCase(UserAnimeList.Domain.Entities.User user)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var userRepositoryBuilder = new UserRepositoryBuilder();
        var userRepository = userRepositoryBuilder.GetById(user).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var tokenRepository = new TokenRepositoryBuilder().Build();

        return new SoftDeleteUseCase(loggedUser, userRepository, unitOfWork,tokenRepository);
    }
}