using System.Net;

namespace UserAnimeList.Exception.Exceptions;

public class RefreshTokenInactiveException : UserAnimeListException
{
    public RefreshTokenInactiveException() : base(ResourceMessagesException.INVALID_SESSION)
    {
    }

    public override IList<string> GetErrorMessages() => [Message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Forbidden;
}