namespace UserAnimeList.Domain.Security.Tokens;


public interface IAccessTokenValidator
{
    public AccessTokenData Validate(string token);
}
