using System.Net;

namespace UserAnimeList.Exception.Exceptions;

public abstract class UserAnimeListException : SystemException
{
    protected UserAnimeListException(string message) : base(message) { }
    
    public abstract IList<string> GetErrorMessages();
    public abstract HttpStatusCode GetStatusCode();
}