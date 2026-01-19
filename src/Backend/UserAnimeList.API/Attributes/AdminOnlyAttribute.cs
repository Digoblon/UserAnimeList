using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserAnimeList.Enums;
using UserAnimeList.Filters;

namespace UserAnimeList.Attributes;

public class AdminOnlyAttribute : TypeFilterAttribute
{
    public AdminOnlyAttribute() : base(typeof(RequireRoleFilter)) { }
    
}
public class AdminOnlyFilter : IAsyncAuthorizationFilter
{
    private readonly AuthenticatedUserFilter _authFilter;
    private readonly RequireRoleFilter _roleFilter;

    public AdminOnlyFilter(AuthenticatedUserFilter authFilter)
    {
        _authFilter = authFilter;
        _roleFilter = new RequireRoleFilter(UserRole.Admin);
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        await _authFilter.OnAuthorizationAsync(context);
        
        if (context.Result is not null)
            return;

        await _roleFilter.OnAuthorizationAsync(context);
    }
}