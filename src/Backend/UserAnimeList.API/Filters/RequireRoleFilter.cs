using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Errors;
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
            var response = ErrorResponseFactory.FromStatusCode(
                context.HttpContext,
                StatusCodes.Status401Unauthorized,
                [ResourceMessagesException.NO_TOKEN]);
            context.Result = new UnauthorizedObjectResult(response);
            return Task.CompletedTask;
        }

        if (tokenData.Role != _requiredRole)
        {
            var response = ErrorResponseFactory.FromStatusCode(
                context.HttpContext,
                StatusCodes.Status403Forbidden,
                [ResourceMessagesException.NO_ACCESS]);
            context.Result = new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        return Task.CompletedTask;
    }
}
