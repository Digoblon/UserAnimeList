using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if(context.Exception is UserAnimeListException userAnimeListException)
            HandleProjectException(userAnimeListException, context);
        else
            ThrowUnknownException(context);
    }

    private static void HandleProjectException(UserAnimeListException myRecipeBookException,ExceptionContext context)
    {
        
        context.HttpContext.Response.StatusCode = (int)myRecipeBookException.GetStatusCode();
        context.Result = new ObjectResult(new ResponseErrorJson(myRecipeBookException.GetErrorMessages()));
    }
    
    private static void ThrowUnknownException(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesException.UNKNOWN_ERROR));
    }
}