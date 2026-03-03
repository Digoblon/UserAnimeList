using System.Net;

namespace UserAnimeList.Exception.Exceptions;

public class InvalidSingingKeyException : UserAnimeListException
{
    public InvalidSingingKeyException() : base(ResourceMessagesException.INVALID_SIGNING_KEY)
    {
    }

    public override IList<string> GetErrorMessages() => [Message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Forbidden;
}