namespace UserAnimeList.Domain.Entities;

public class Studio : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string NameNormalized { get; set; } = string.Empty;
    public string Description { get; set; } =  string.Empty;
}