using UserAnimeList.Communication.Enums;

namespace UserAnimeList.Communication.Responses;

public class ResponseShortAnimeJson
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AnimeStatus Status { get; set; }
    public AnimeType Type { get; set; }
    public string AiredFrom { get; set; } = "?";
    public string AiredUntil { get; set; } = "?";
}