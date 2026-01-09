namespace UserAnimeList.Domain.Security.Tokens;

public interface IAccessTokenValidator
{
    public Guid ValidateAndGetId(string token);
}