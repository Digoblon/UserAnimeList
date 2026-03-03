using System.Net;

namespace UserAnimeList.Exception.Exceptions;

public class InvalidSearchException : UserAnimeListException
{
    public InvalidSearchException(string message) : base(message)
    {
    }

    public override IList<string> GetErrorMessages() => [Message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
}