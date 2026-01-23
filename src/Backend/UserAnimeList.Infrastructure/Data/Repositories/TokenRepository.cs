using Microsoft.EntityFrameworkCore;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.Token;

namespace UserAnimeList.Infrastructure.Data.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly UserAnimeListDbContext _dbContext;
    
    public TokenRepository(UserAnimeListDbContext dbContext) =>     _dbContext = dbContext;
    
    public async Task<RefreshToken?> Get(string refreshToken)
    {
        return await _dbContext
            .RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(token => token.Token.Equals(refreshToken));
    }

    public async Task SaveNewRefreshToken(RefreshToken refreshToken) => await _dbContext.RefreshTokens.AddAsync(refreshToken);
    public void RevokeRefreshToken(RefreshToken refreshToken)
    {
         _dbContext
            .RefreshTokens
            .Update(refreshToken);
            
    }
    
    public async Task RevokeAllForUser(Guid userId)
    {
        var utcNow = DateTime.UtcNow;

        try
        {
            await _dbContext.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedOn == null)
                .ExecuteUpdateAsync(setters =>
                    setters
                        .SetProperty(t => t.RevokedOn, utcNow)
                        .SetProperty(t => t.IsActive, false)
                );
        }
        catch (InvalidOperationException)
        {
            // Fallback para providers que não suportam ExecuteUpdate (ex: InMemory)
            // Utilizado nos testes de integração
            var tokens = await _dbContext.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedOn == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokedOn = utcNow;
                token.IsActive = false;
            }

        }
    }
}