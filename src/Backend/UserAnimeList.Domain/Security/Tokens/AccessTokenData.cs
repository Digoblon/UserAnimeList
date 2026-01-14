namespace UserAnimeList.Domain.Security.Tokens;

public record AccessTokenData(
    Guid UserId,
    int TokenVersion);