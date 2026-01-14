using System.Net;

namespace UserAnimeList.Exception.Exceptions;

public class UnknownUserException : UserAnimeListException
{
    public UnknownUserException(string message) : base(message)
    {
    }

    public override IList<string> GetErrorMessages() => [Message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Forbidden;
}