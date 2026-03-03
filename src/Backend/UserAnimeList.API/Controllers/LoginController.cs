using Microsoft.AspNetCore.Mvc;
using UserAnimeList.Application.UseCases.Login.DoLogin;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Controllers;

public class LoginController : UserAnimeListBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisteredUserJson),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson),StatusCodes.Status401Unauthorized)]
    public async  Task<IActionResult> Login(
        [FromServices] IDoLoginUseCase useCase,
        [FromBody] RequestLoginJson request)
    {
        var response = await useCase.Execute(request);

        return Ok(response);
    }
}
