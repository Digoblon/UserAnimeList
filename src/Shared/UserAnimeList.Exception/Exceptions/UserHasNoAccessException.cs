using System.Net;

namespace UserAnimeList.Exception.Exceptions;

public class UserHasNoAccessException : UserAnimeListException
{
    public UserHasNoAccessException() : base(ResourceMessagesException.USER_HAS_NO_ACCESS)
    {
    }

    public override IList<string> GetErrorMessages() => [Message];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Forbidden;
}