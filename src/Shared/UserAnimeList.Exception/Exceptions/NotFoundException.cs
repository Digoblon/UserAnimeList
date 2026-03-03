using System.Net;

namespace UserAnimeList.Exception.Exceptions;

public class NotFoundException : UserAnimeListException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public override IList<string> GetErrorMessages() => [Message];

    public override HttpStatusCode GetStatusCode() =>  HttpStatusCode.NotFound;
}