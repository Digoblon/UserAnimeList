using Microsoft.AspNetCore.Mvc;
using UserAnimeList.Application.UseCases.Token.RefreshToken;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Controllers;

public class TokenController : UserAnimeListBaseController
{
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ResponseTokensJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken(
        [FromServices] IUseRefreshTokenUseCase useCase,
        [FromBody] RequestNewTokenJson request)
    {
        var response = await useCase.Execute(request);

        return Ok(response);
    }
}