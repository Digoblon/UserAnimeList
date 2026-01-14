using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Application.UseCases.User.ChangePassword;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.User.ChangePassword;

public class ChangePasswordUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        (var user, var password) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();
        request.Password = password;

        var useCase = CreateUseCase(user);
        
        Func<Task> act = async () => await useCase.Execute(request);

        await act();
    }

    [Fact]
    public async Task Error_NewPassword_Empty()
    {
        (var user, var password) = UserBuilder.Build();

        var request = new RequestChangePasswordJson
        {
            Password = password,
            NewPassword = string.Empty,
            ConfirmNewPassword = string.Empty
            
        };
        
        var useCase = CreateUseCase(user);
        
        Func<Task> act = async () => { await useCase.Execute(request); };
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.PASSWORD_EMPTY,  exception.GetErrorMessages().First());
    }
    
    [Fact]
    public async Task Error_CurrentPassword_Different()
    {
        (var user, var password) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();
        
        var useCase = CreateUseCase(user);
        
        Func<Task> act = async () => { await useCase.Execute(request); };
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.PASSWORD_DIFFERENT_CURRENT_PASSWORD,  exception.GetErrorMessages().First());
    }
    
    [Fact]
    public async Task Error_Password_Not_Match()
    {
        (var user, var password) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Build();
        request.Password = password;
        request.ConfirmNewPassword = request.NewPassword + "A";
        
        var useCase = CreateUseCase(user);
        
        Func<Task> act = async () => { await useCase.Execute(request); };
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.PASSWORDS_NOT_MATCH,  exception.GetErrorMessages().First());
    }
    
    
    private static ChangePasswordUseCase CreateUseCase(UserAnimeList.Domain.Entities.User user, string? email = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var userRepositoryBuilder = new UserRepositoryBuilder();
        var userRepository = userRepositoryBuilder.GetById(user).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var passwordEncrypter = PasswordEncrypterBuilder.Build();
        var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();
        
        
        return new ChangePasswordUseCase(loggedUser,userRepository,unitOfWork,passwordEncrypter,accessTokenGenerator);
    }
}