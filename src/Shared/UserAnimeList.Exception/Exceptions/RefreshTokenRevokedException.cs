using System.Net;

namespace UserAnimeList.Exception.Exceptions;

public class RefreshTokenRevokedException : UserAnimeListException
{
    public RefreshTokenRevokedException() : base(ResourceMessagesException.INVALID_SESSION)
    {
    }

    public override IList<string> GetErrorMessages() => [Message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
}