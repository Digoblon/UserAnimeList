namespace UserAnimeList.Domain.Entities;

public class RefreshToken : EntityBase
{
    public required string Token { get; set; } = string.Empty;
    public required Guid  UserId { get; set; }
    public DateTime? RevokedOn { get; set; }
    public User User { get; set; } = null!;
    
}