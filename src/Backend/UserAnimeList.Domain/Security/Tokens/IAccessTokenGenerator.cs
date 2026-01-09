namespace UserAnimeList.Domain.Security.Tokens;

public interface IAccessTokenGenerator
{
    public string Generate(Guid id);
}