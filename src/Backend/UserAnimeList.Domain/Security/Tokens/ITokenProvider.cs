namespace UserAnimeList.Domain.Security.Tokens;

public interface ITokenProvider
{
    public AccessTokenData Value();
}