using System.Net;

namespace UserAnimeList.Exception.Exceptions;

public class RefreshTokenNotFoundException : UserAnimeListException
{
    public RefreshTokenNotFoundException() : base(ResourceMessagesException.EXPIRED_SESSION)
    {
    }

    public override IList<string> GetErrorMessages() => [Message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
}