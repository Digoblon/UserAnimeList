namespace UserAnimeList.Domain.Repositories.Token;

public interface ITokenRepository
{
    Task<Entities.RefreshToken?> Get(string refreshToken);
    Task SaveNewRefreshToken(Entities.RefreshToken refreshToken);
    public void RevokeRefreshToken(Entities.RefreshToken refreshToken);
    public Task RevokeAllForUser(Guid userId);
}