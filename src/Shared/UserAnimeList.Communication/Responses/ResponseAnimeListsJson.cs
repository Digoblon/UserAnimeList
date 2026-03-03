namespace UserAnimeList.Communication.Responses;

public class ResponseAnimeListsJson
{
    public IList<ResponseShortAnimeListEntryJson> Lists { get; set; } = []; 
}