using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Application.UseCases.User.Register;
using UserAnimeList.Domain.Extensions;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.User.Register;

public class RegisterUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        var useCase = CreateUseCase();
        
        var result = await useCase.Execute(request);

        Assert.NotNull(result);
        Assert.Equal(request.UserName, result.UserName);
        
        //result.Should().NotBeNull();
        //result.Tokens.Should().NotBeNull();
        //result.Name.Should().Be(request.Name);
        //result.Tokens.AccessToken.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task Error_Email_Already_Registered()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        
        var useCase = CreateUseCase(request.Email);
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();

        Assert.Single(errors);
        Assert.Contains(ResourceMessagesException.EMAIL_ALREADY_REGISTERED, errors);

        
        //(await act.Should().ThrowAsync<ErrorOnValidationException>()).Where(e => e.GetErrorMessages().Count == 1 && e.GetErrorMessages().Contains(ResourceMessagesException.EMAIL_ALREADY_REGISTERED));
        
    }
    
    [Fact]
    public async Task Error_UserName_Empty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.UserName = string.Empty;
        
        var useCase = CreateUseCase();
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();

        Assert.Single(errors);
        Assert.Contains(ResourceMessagesException.NAME_EMPTY, errors);
        
    }

    private static RegisterUserUseCase CreateUseCase(string? email = null)
    {
        var mapper = MapperBuilder.Build();
        var passwordEncrypter = PasswordEncrypterBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var userRepositoryBuilder = new UserRepositoryBuilder();
        var userRepository = userRepositoryBuilder.Build();
        var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();
        //var refreshTokenGenerator = RefreshTokenGeneratorBuilder.Build();
        //var tokenRepository = new TokenRepositoryBuilder().Build();

        if (email.NotEmpty())
            userRepositoryBuilder.ExistActiveUserWithEmail(email);

        return new RegisterUserUseCase(mapper, passwordEncrypter,userRepository, unitOfWork,accessTokenGenerator);

    }
}