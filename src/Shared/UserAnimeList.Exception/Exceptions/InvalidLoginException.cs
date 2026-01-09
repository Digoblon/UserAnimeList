using System.Net;

namespace UserAnimeList.Exception.Exceptions;

public class InvalidLoginException : UserAnimeListException
{
    public InvalidLoginException() : base(ResourceMessagesException.LOGIN_OR_PASSWORD_INVALID)
    {
    }

    public override IList<string> GetErrorMessages() => [Message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
}