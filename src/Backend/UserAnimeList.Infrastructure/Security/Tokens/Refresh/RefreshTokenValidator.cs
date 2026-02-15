using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Domain.ValueObjects;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Infrastructure.Security.Tokens.Refresh;

public class RefreshTokenValidator : IRefreshTokenValidator
{
    public void Validate(RefreshToken? refreshToken)
    {
        if (refreshToken is null)
            throw new RefreshTokenNotFoundException();

        if (refreshToken.CreatedOn > DateTime.UtcNow)
            throw new RefreshTokenInvalidTimestampException();

        if (refreshToken.RevokedOn is not null)
            throw new RefreshTokenRevokedException();

        var validUntil = refreshToken.CreatedOn
            .AddDays(UserAnimeListConstants.RefreshTokenExpirationDays);

        if (validUntil <= DateTime.UtcNow)
            throw new RefreshTokenExpiredException();
    }
}