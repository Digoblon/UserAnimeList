using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.User.Update;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.User.Update;

public class UpdateUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUseCase(user);
        
        Func<Task> act = async () => await useCase.Execute(request);

        await act();
        
        Assert.Equal(request.UserName, user.UserName);
        Assert.Equal(request.Email, user.Email);
    }
    
    [Fact]
    public async Task Error_Name_Empty()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();
        request.UserName = string.Empty;

        var useCase = CreateUseCase(user);
        
        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.NAME_EMPTY, exception.GetErrorMessages().FirstOrDefault());
        
        Assert.NotEqual(request.UserName,user.UserName);
        Assert.NotEqual(request.Email,user.Email);
    }
    
    [Fact]
    public async Task Error_Email_Already_Registered()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUseCase(user,request.Email);
        
        Func<Task> act = async () => await useCase.Execute(request);
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.EMAIL_ALREADY_REGISTERED, exception.GetErrorMessages().FirstOrDefault());
        
        Assert.NotEqual(request.UserName,user.UserName);
        Assert.NotEqual(request.Email,user.Email);
    }


    private static UpdateUserUseCase CreateUseCase(UserAnimeList.Domain.Entities.User user, string? email = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var userRepositoryBuilder = new UserRepositoryBuilder();
        var userRepository = userRepositoryBuilder.GetById(user).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        
        if(!string.IsNullOrEmpty(email))
            userRepositoryBuilder.ExistActiveUserWithEmail(email);

        return new UpdateUserUseCase(loggedUser, userRepository, unitOfWork);
    }
}