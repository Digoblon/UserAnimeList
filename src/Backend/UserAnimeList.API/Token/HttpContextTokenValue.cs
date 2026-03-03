using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Token;

public class HttpContextTokenValue : ITokenProvider
{
    private const string TokenDataKey = "TokenData";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextTokenValue(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public AccessTokenData Value()
    {
        var httpContext = _httpContextAccessor.HttpContext
                          ?? throw new UnauthorizedException(ResourceMessagesException.NO_TOKEN);

        if (!httpContext.Items.TryGetValue(TokenDataKey, out var value))
            throw new UnauthorizedException(ResourceMessagesException.NO_TOKEN);

        if (value is not AccessTokenData tokenData)
            throw new UnauthorizedException(ResourceMessagesException.NO_TOKEN);
        
        return tokenData;
    }
}