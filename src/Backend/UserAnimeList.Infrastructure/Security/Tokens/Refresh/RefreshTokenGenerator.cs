using System.Security.Cryptography;
using UserAnimeList.Domain.Security.Tokens;

namespace UserAnimeList.Infrastructure.Security.Tokens.Refresh;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string Generate()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes).Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
}