using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Application.UseCases.Login.DoLogin;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.User.Login.DoLogin;

public class DoLoginUseCaseTest
{
    [Fact]
    public async Task Success_Email()
    {
        var (user, password) = UserBuilder.Build();
        var login = new RequestLoginJson()
        {
            Login = user.Email,
            Password = password
        };
        var useCase = CreateUseCase(login.Login,user);

        var result = await useCase.Execute(login);
        
        Assert.NotNull(result);
        
        Assert.NotNull(result.UserName);
        Assert.NotEmpty(result.UserName);
        Assert.Equal(result.UserName, user.UserName);
        
        Assert.NotNull(result.Tokens);
        Assert.NotNull(result.Tokens.AccessToken);
        Assert.NotEmpty(result.Tokens.AccessToken);
        
        Assert.NotNull(result.Tokens.RefreshToken);
        Assert.NotEmpty(result.Tokens.RefreshToken);
    }
    
    [Fact]
    public async Task Success_UserName()
    {
        var (user, password) = UserBuilder.Build();
        var login = new RequestLoginJson()
        {
            Login = user.UserName,
            Password = password
        };
        var useCase = CreateUseCase(login.Login,user);

        var result = await useCase.Execute(login);
        
        Assert.NotNull(result);
        
        Assert.NotNull(result.UserName);
        Assert.NotEmpty(result.UserName);
        Assert.Equal(result.UserName, user.UserName);
        
        Assert.NotNull(result.Tokens);
        Assert.NotNull(result.Tokens.AccessToken);
        Assert.NotEmpty(result.Tokens.AccessToken);
        
        Assert.NotNull(result.Tokens.RefreshToken);
        Assert.NotEmpty(result.Tokens.RefreshToken);
    }

    [Fact]
    public async Task Error_Invalid_User()
    {
        var request = RequestLoginJsonBuilder.Build();

        var useCase = CreateUseCase("");

        Func<Task> act = async () => {await useCase.Execute(request); };

        var exception = await Assert.ThrowsAsync<InvalidLoginException>(act);
        Assert.Equal(exception.GetErrorMessages().FirstOrDefault(),ResourceMessagesException.LOGIN_OR_PASSWORD_INVALID);
        
        //await act.Should().ThrowAsync<InvalidLoginException>().Where(e => e.Message.Equals(ResourceMessagesException.EMAIL_OR_PASSWORD_INVALID));
    }

    private static DoLoginUseCase CreateUseCase(string login,UserAnimeList.Domain.Entities.User? user = null)
    {
        var passwordEncrypter = PasswordEncrypterBuilder.Build();
        var userRepositoryBuilder = new UserRepositoryBuilder();
        var userRepository = userRepositoryBuilder.Build();
        var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();
        var refreshTokenGenerator = RefreshTokenGeneratorBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var tokenRepository = new TokenRepositoryBuilder().Build();

        if (user is not null)
            userRepositoryBuilder.GetByLogin(user,login);

        return new DoLoginUseCase(userRepository, passwordEncrypter, accessTokenGenerator,refreshTokenGenerator, tokenRepository, unitOfWork);
    }
}