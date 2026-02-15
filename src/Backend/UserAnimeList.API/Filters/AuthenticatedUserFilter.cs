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
    private const string TokenDataKey = "TokenData";
    
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
            var tokenData = _accessTokenValidator.Validate(token);

            var user = await _repository.GetById(tokenData.UserId);

            if (user is null || !user.IsActive)
                throw new UnknownUserException(ResourceMessagesException.NO_ACCESS);

            if (user.TokenVersion != tokenData.TokenVersion)
                throw new SecurityTokenException(ResourceMessagesException.WRONG_TOKEN_VERSION);

            context.HttpContext.Items[TokenDataKey] = tokenData;
        }
        catch (SecurityTokenExpiredException)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesException.TOKEN_EXPIRED)
            {
                TokenIsExpired = true
            });
        }
        catch (SecurityTokenException)
        {
            context.Result =
                new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesException.INVALID_TOKEN));
        }
        catch (SecurityTokenMalformedException)
        {
            context.Result =
                new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesException.INVALID_TOKEN));
        }
        catch (UserAnimeListException ex)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ex.Message))
            {
                StatusCode = (int)ex.GetStatusCode()
            };
        }

    }

    private static string TokenOnRequest(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            throw new UnauthorizedException(ResourceMessagesException.NO_TOKEN);

        var authorization = authorizationHeader.ToString();

        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedException(ResourceMessagesException.INVALID_TOKEN);

        var token = authorization["Bearer ".Length..].Trim();

        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedException(ResourceMessagesException.INVALID_TOKEN);

        return token;
    }
}