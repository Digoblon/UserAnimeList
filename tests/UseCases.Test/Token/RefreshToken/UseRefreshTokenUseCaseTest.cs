using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Tokens;
using UserAnimeList.Application.UseCases.Token.RefreshToken;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.ValueObjects;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Token.RefreshToken;

public class UseRefreshTokenUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        (var user, _) = UserBuilder.Build();
        var refreshToken = RefreshTokenBuilder.Build(user);
        
        var useCase = CreateUseCase(refreshToken);

        var result = await useCase.Execute(new RequestNewTokenJson
        {
            RefreshToken = refreshToken.Token
        });
        
        Assert.NotNull(result);
        Assert.NotNull(result.RefreshToken);
        Assert.NotNull(result.AccessToken);
        Assert.NotEqual(refreshToken.Token, result.RefreshToken);
    }

    [Fact]
    public async Task Error_RefreshToken_Not_Found()
    {
        var useCase = CreateUseCase();

        var act = async () => await useCase.Execute(new RequestNewTokenJson
        {
            RefreshToken = string.Empty
        });

        var exception = await Assert.ThrowsAsync<RefreshTokenNotFoundException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.EXPIRED_SESSION, exception.GetErrorMessages().First());
    }
    

    private static UseRefreshTokenUseCase CreateUseCase(UserAnimeList.Domain.Entities.RefreshToken? refreshToken = null)
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();
        var refreshTokenGenerator = RefreshTokenGeneratorBuilder.Build();
        var tokenRepository = new TokenRepositoryBuilder().Get(refreshToken).Build();
        var refreshTokenValidator = RefreshTokenValidatorBuilder.Build();

        return new UseRefreshTokenUseCase(tokenRepository,unitOfWork,accessTokenGenerator,refreshTokenValidator, refreshTokenGenerator);
    }
}
