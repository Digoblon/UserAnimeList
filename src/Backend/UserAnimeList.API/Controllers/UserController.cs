using Microsoft.AspNetCore.Mvc;
using UserAnimeList.Application.UseCases.User.Profile;
using UserAnimeList.Application.UseCases.User.Register;
using UserAnimeList.Attributes;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

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
        
        [HttpGet]
        [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
        [AuthenticatedUser]
        public async Task<IActionResult> GetUserProfile([FromServices] IGetUserProfileUseCase useCase)
        {
            var result = await useCase.Execute();
            
            return Ok(result);
        }
    }
    
    
}
