using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Domain.Security.Tokens;

public interface IRefreshTokenValidator
{
    public void Validate(RefreshToken refreshToken);
}