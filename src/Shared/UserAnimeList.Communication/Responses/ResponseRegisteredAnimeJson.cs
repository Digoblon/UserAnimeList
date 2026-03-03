namespace UserAnimeList.Communication.Responses;

public class ResponseRegisteredAnimeJson
{
    public Guid AnimeId { get; set; }
    public string Name { get; set; } =  string.Empty;
}