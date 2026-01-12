using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Filters;

public class AuthenticatedUserFilter : IAsyncAuthorizationFilter
{
    private const string UserIdKey = "UserId";
    
    private readonly IAccessTokenValidator _accessTokenValidator;
    private readonly IUserRepository _repository;

    public AuthenticatedUserFilter(IAccessTokenValidator accessTokenValidator, IUserRepository repository)
    {
        _accessTokenValidator = accessTokenValidator;
        _repository = repository;
    }
    
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            var token = TokenOnRequest(context);
            var id = _accessTokenValidator.ValidateAndGetId(token);
            var exist = await _repository.ExistActiveUserWithId(id);

            if (!exist)
                throw new UnauthorizedException(ResourceMessagesException.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE);
            
            context.HttpContext.Items[UserIdKey] = id;
        }
        catch (SecurityTokenExpiredException)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson("Token is expired")
            {
                TokenIsExpired = true
            });
        }
        catch (UserAnimeListException ex)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ex.Message));
        }
        catch
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesException.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE));
        }

    }

    private static string TokenOnRequest(AuthorizationFilterContext context)
    {
        var authentication = context.HttpContext.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authentication))
            throw new UnauthorizedException(ResourceMessagesException.NO_TOKEN);

        return authentication["Bearer ".Length..].Trim();
    }
}