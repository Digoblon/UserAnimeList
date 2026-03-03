using System.Net;

namespace UserAnimeList.Exception.Exceptions;

public class RefreshTokenExpiredException : UserAnimeListException
{
    public RefreshTokenExpiredException() : base(ResourceMessagesException.EXPIRED_SESSION)
    {
    }

    public override IList<string> GetErrorMessages() => [Message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
}