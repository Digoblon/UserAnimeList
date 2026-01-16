using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Infrastructure.Security.Tokens.Refresh;

namespace CommonTestUtilities.Tokens;

public class RefreshTokenValidatorBuilder
{
    public static IRefreshTokenValidator Build() => new RefreshTokenValidator();
}