using UserAnimeList.Communication.Responses;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Errors;

public static class ErrorResponseFactory
{
    public static ResponseErrorJson FromException(HttpContext httpContext, UserAnimeListException exception)
    {
        return FromStatusCode(httpContext, (int)exception.GetStatusCode(), exception.GetErrorMessages());
    }

    public static ResponseErrorJson FromStatusCode(
        HttpContext httpContext,
        int statusCode,
        IList<string> errors,
        bool tokenIsExpired = false)
    {
        var normalizedErrors = errors.Where(error => !string.IsNullOrWhiteSpace(error)).Distinct().ToList();

        if (normalizedErrors.Count == 0)
            normalizedErrors = [DefaultMessage(statusCode)];

        var response = new ResponseErrorJson(normalizedErrors)
        {
            Code = ResolveCode(statusCode, normalizedErrors, tokenIsExpired),
            Message = normalizedErrors.First(),
            TraceId = httpContext.TraceIdentifier,
            TokenIsExpired = tokenIsExpired
        };

        return response;
    }

    public static ResponseErrorJson Unknown(HttpContext httpContext)
    {
        return FromStatusCode(
            httpContext,
            StatusCodes.Status500InternalServerError,
            [ResourceMessagesException.UNKNOWN_ERROR]);
    }

    private static string ResolveCode(int statusCode, IList<string> errors, bool tokenIsExpired)
    {
        if (tokenIsExpired || errors.Contains(ResourceMessagesException.TOKEN_EXPIRED))
            return "token_expired";

        if (errors.Contains(ResourceMessagesException.INVALID_TOKEN) || errors.Contains(ResourceMessagesException.NO_TOKEN))
            return "invalid_token";

        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "validation_error",
            StatusCodes.Status401Unauthorized => "unauthorized",
            StatusCodes.Status403Forbidden => "forbidden",
            StatusCodes.Status404NotFound => "not_found",
            StatusCodes.Status500InternalServerError => "internal_error",
            _ => "error"
        };
    }

    private static string DefaultMessage(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status500InternalServerError => ResourceMessagesException.UNKNOWN_ERROR,
            StatusCodes.Status401Unauthorized => ResourceMessagesException.INVALID_TOKEN,
            StatusCodes.Status403Forbidden => ResourceMessagesException.NO_ACCESS,
            StatusCodes.Status404NotFound => ResourceMessagesException.USER_NOT_FOUND,
            _ => ResourceMessagesException.INVALID_REQUEST_BODY
        };
    }
}
