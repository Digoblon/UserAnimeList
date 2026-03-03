using UserAnimeList.Domain.Entities;
using UserAnimeList.Enums;

namespace UserAnimeList.Domain.Security.Tokens;

public interface IAccessTokenGenerator
{
    public string Generate(Guid id, int tokenVersion, UserRole role);
}