using Microsoft.AspNetCore.Mvc;
using UserAnimeList.Application.UseCases.User.Register;
using UserAnimeList.Communication.Requests;

namespace UserAnimeList.Controllers
{
    public class UserController : UserAnimeListBaseController
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Register(
            [FromServices]IRegisterUserUseCase useCase,
            [FromBody]RequestRegisterUserJson request)
        {
            var result = await useCase.Execute(request);
            
            return Created(string.Empty, result);
        }
    }
}
