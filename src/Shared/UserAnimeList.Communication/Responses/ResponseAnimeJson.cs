using UserAnimeList.Communication.Enums;

namespace UserAnimeList.Communication.Responses;

public class ResponseAnimeJson
{
    public Guid Id { get; set; }
    public string Name { get; set; } =  string.Empty;
    public double? Score { get; set; }
    public string? Synopsis { get; set; } =  string.Empty;
    public int? Episodes { get; set; }
    public IList<Guid> Genres { get; set; } = [];
    public IList<Guid> Studios { get; set; } = [];
    public AnimeStatus Status { get; set; }
    public SourceType Source { get; set; }
    public AnimeType Type { get; set; }
    public DateOnly? AiredFrom { get; set; }
    public DateOnly? AiredUntil { get; set; }
    public string Premiered  {get; set; } = "?";
}