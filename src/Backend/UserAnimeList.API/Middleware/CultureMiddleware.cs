using System.Globalization;
using UserAnimeList.Domain.Extensions;
using UserAnimeList.Domain.ValueObjects;

namespace UserAnimeList.Middleware;

public class CultureMiddleware
{
    private readonly RequestDelegate _next;
    
    public CultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context)
    {
        var supportedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
        
        var requestedCulture = context.Request.Headers.AcceptLanguage.FirstOrDefault();
        var cultureInfo = new CultureInfo(UserAnimeListConstants.DefaultCulture);
        
        if(requestedCulture.NotEmpty() && supportedLanguages.Exists(c => c.Name.Equals(requestedCulture)))
            cultureInfo = new CultureInfo(requestedCulture!);
        
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
        
        await _next(context);
    }
}