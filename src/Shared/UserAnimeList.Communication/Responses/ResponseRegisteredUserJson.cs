namespace UserAnimeList.Communication.Responses;

public class ResponseRegisteredUserJson 
{
    public string UserName { get; set; } =  string.Empty;
    public ResponseTokensJson Tokens { get; set; } = null!;
}