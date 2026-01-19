using Microsoft.AspNetCore.Http;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Domain.Services.LoggedUser;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Infrastructure.Services.LoggedUser;

public class LoggedUser : ILoggedUser
{
    //private const string UserIdKey = "UserId";
    private const string TokenDataKey = "TokenData";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _repository;

    public LoggedUser(
        IHttpContextAccessor httpContextAccessor,
        IUserRepository repository)
    {
        _httpContextAccessor = httpContextAccessor;
        _repository = repository;
    }

    public async Task<User> User()
    {
        var httpContext = _httpContextAccessor.HttpContext
                          ?? throw new UnauthorizedException(ResourceMessagesException.NO_TOKEN);

        if (!httpContext.Items.TryGetValue(TokenDataKey, out var value))
            throw new UnauthorizedException(ResourceMessagesException.NO_TOKEN);

        if (value is not AccessTokenData tokenData)
            throw new UnauthorizedException(ResourceMessagesException.NO_TOKEN);
        
        //var userId = (Guid)value;

        var user = await _repository.GetById(tokenData.UserId)
                   ?? throw new UnauthorizedException(ResourceMessagesException.USER_NOT_FOUND);

        return user;
            
    }
}