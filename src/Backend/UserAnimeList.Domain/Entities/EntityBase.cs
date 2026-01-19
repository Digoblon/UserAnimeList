namespace UserAnimeList.Domain.Entities;

public class EntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } =  true;
    public DateTime? DeletedOn { get; set; } = null;
}