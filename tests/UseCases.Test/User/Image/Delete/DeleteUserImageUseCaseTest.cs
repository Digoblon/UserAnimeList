using CommonTestUtilities.Entities;
using CommonTestUtilities.FileStorage;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using UserAnimeList.Application.UseCases.User.Image.Delete;

namespace UseCases.Test.User.Image.Delete;

public class DeleteUserImageUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => await useCase.Execute();

        await act();
    }
    
    private static DeleteUserImageUseCase CreateUseCase(UserAnimeList.Domain.Entities.User user)
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var userRepository = new UserRepositoryBuilder().Build();
        var fileStorageBuilder = new FileStorageBuilder();
        var fileStorage = fileStorageBuilder.Save().Build();
        
        return new DeleteUserImageUseCase(loggedUser,userRepository,fileStorage,unitOfWork);

    }
}