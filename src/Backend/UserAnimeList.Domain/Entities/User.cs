using UserAnimeList.Enums;

namespace UserAnimeList.Domain.Entities;

public class User : EntityBase
{
    public string UserName  { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TokenVersion { get; private set; } = 1;
    public UserRole Role { get; set; } = UserRole.User;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    
    public void IncrementTokenVersion()
    {
        TokenVersion++;
    }
}