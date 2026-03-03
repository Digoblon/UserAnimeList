namespace UserAnimeList.Communication.Responses;

public class ResponseErrorJson
{
    public string Code { get; set; }
    public string Message { get; set; }
    public IList<string> Errors { get; private set; }
    public string TraceId { get; set; }
    public bool TokenIsExpired { get; set; }

    public ResponseErrorJson(IList<string> errors)
    {
        Errors = errors;
        Message = errors.FirstOrDefault() ?? string.Empty;
        Code = string.Empty;
        TraceId = string.Empty;
    }

    public ResponseErrorJson(string error)
    {
        Errors = new List<string> { error };
        Message = error;
        Code = string.Empty;
        TraceId = string.Empty;
    }
}
