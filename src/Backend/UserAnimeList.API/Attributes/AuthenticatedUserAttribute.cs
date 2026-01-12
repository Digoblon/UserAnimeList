using Microsoft.AspNetCore.Mvc;
using UserAnimeList.Filters;

namespace UserAnimeList.Attributes;

public class AuthenticatedUserAttribute: TypeFilterAttribute
{
    public AuthenticatedUserAttribute() : base(typeof(AuthenticatedUserFilter))
    {
    }
}