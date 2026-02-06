using CommonTestUtilities.Entities;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.ValueObjects;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;
using UserAnimeList.Infrastructure.Security.Tokens.Refresh;

namespace Validators.Test.Token.RefreshTokenTest;

public class RefreshTokenValidatorTest
{
    [Fact]
    public void Success()
    {
        (var user, _) = UserBuilder.Build(); var refreshToken = RefreshTokenBuilder.Build(user);
        
        //refreshToken.CreatedOn = DateTime.UtcNow.AddDays(- UserAnimeListConstants.RefreshTokenExpirationDays + 2);

        var validator = new RefreshTokenValidator();

        var act = () => validator.Validate(refreshToken);

        act();
    }

    [Fact]
    public void Error_Token_Null()
    {
        var validator = new RefreshTokenValidator();
        
        RefreshToken refreshToken = null!;

        var act = () => validator.Validate(refreshToken);

        var exception = Assert.Throws<RefreshTokenNotFoundException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.EXPIRED_SESSION, exception.GetErrorMessages().First());
    }
    
    [Fact]
    public void Error_Token_Invalid_TimeStamp()
    {
        (var user, _) = UserBuilder.Build(); 
        var refreshToken = RefreshTokenBuilder.Build(user);
        refreshToken.CreatedOn = DateTime.UtcNow.AddDays(UserAnimeListConstants.RefreshTokenExpirationDays + 2);
        var validator = new RefreshTokenValidator();

        var act = () => validator.Validate(refreshToken);

        var exception = Assert.Throws<RefreshTokenInvalidTimestampException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.INVALID_SESSION, exception.GetErrorMessages().First());
    }

    [Fact]
    public void Error_Token_Revoked()
    {
        (var user, _) = UserBuilder.Build(); 
        var refreshToken = RefreshTokenBuilder.Build(user);
        
        //refreshToken.CreatedOn = DateTime.UtcNow.AddDays(- UserAnimeListConstants.RefreshTokenExpirationDays + 2);
        refreshToken.RevokedOn = refreshToken.CreatedOn.AddDays(2);

        var validator = new RefreshTokenValidator();

        var act = () => validator.Validate(refreshToken);

        var exception = Assert.Throws<RefreshTokenRevokedException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.INVALID_SESSION, exception.GetErrorMessages().First());
    }
    
    [Fact]
    public void Error_Token_Expired()
    {
        (var user, _) = UserBuilder.Build(); var refreshToken = RefreshTokenBuilder.Build(user);
        
        refreshToken.CreatedOn = DateTime.UtcNow.AddDays(- UserAnimeListConstants.RefreshTokenExpirationDays - 5);

        var validator = new RefreshTokenValidator();

        var act = () => validator.Validate(refreshToken);

        var exception = Assert.Throws<RefreshTokenExpiredException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.EXPIRED_SESSION, exception.GetErrorMessages().First());
    }
}