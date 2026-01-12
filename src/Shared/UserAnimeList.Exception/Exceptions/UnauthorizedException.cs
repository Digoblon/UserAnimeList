using System.Net;

namespace UserAnimeList.Exception.Exceptions;

public class UnauthorizedException : UserAnimeListException
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public override IList<string> GetErrorMessages() => [Message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
}