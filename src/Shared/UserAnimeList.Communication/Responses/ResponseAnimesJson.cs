namespace UserAnimeList.Communication.Responses;

public class ResponseAnimesJson
{
    public IList<ResponseShortAnimeJson> Animes { get; set; } = [];
}