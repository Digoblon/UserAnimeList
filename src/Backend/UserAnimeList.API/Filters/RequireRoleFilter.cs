using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Enums;
using UserAnimeList.Exception;

namespace UserAnimeList.Filters;

public class RequireRoleFilter: IAsyncAuthorizationFilter
{
    private const string TokenDataKey = "TokenData";
    private readonly UserRole _requiredRole;

    public RequireRoleFilter(UserRole requiredRole)
    {
        _requiredRole = requiredRole;
    }

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Items.TryGetValue(TokenDataKey, out var value) || value is not AccessTokenData tokenData)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesException.NO_TOKEN));
            return Task.CompletedTask;
        }

        if (tokenData.Role != _requiredRole)
        {
            context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesException.NO_ACCESS))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        return Task.CompletedTask;
    }
}
