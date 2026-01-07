using UserAnimeList.Communication.Responses;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UserAnimeListException ex)
        {
            await HandleDomainException(context, ex);
        }
        catch (System.Exception)
        {
            await HandleUnknownException(context);
        }
    }

    private static async Task HandleDomainException(
        HttpContext context,
        UserAnimeListException exception)
    {
        context.Response.StatusCode = (int)exception.GetStatusCode();
        context.Response.ContentType = "application/json";

        var response = new ResponseErrorJson(exception.GetErrorMessages());

        await context.Response.WriteAsJsonAsync(response);
    }

    private static async Task HandleUnknownException(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new ResponseErrorJson(
            ResourceMessagesException.UNKNOWN_ERROR);

        await context.Response.WriteAsJsonAsync(response);
    }
}