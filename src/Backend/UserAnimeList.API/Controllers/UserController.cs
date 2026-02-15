using Microsoft.AspNetCore.Mvc;
using UserAnimeList.Application.UseCases.User.ChangePassword;
using UserAnimeList.Application.UseCases.User.Delete.SoftDelete;
using UserAnimeList.Application.UseCases.User.Image;
using UserAnimeList.Application.UseCases.User.Image.Delete;
using UserAnimeList.Application.UseCases.User.Image.Update;
using UserAnimeList.Application.UseCases.User.Profile;
using UserAnimeList.Application.UseCases.User.Register;
using UserAnimeList.Application.UseCases.User.Update;
using UserAnimeList.Attributes;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Filters;

namespace UserAnimeList.Controllers;

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
        
    [ServiceFilter(typeof(AbsoluteImageUrlFilter))]
    [HttpPut("me/image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [AuthenticatedUser]
    public async Task<IActionResult> UpdateImage(
        [FromServices]IUpdateUserImageUseCase useCase,
        [FromForm]RequestUpdateImageFormData request)
    {
        var response = await useCase.Execute(request);
        
        return Ok(response);
    }
    
    [HttpDelete("me/image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AuthenticatedUser]
    public async Task<IActionResult> DeleteImage(
        [FromServices]IDeleteUserImageUseCase useCase)
    {
        await useCase.Execute();
            
        return NoContent();
    }
        
    [ServiceFilter(typeof(AbsoluteImageUrlFilter))]
    [HttpGet]
    [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
    [AuthenticatedUser]
    public async Task<IActionResult> GetUserProfile([FromServices] IGetUserProfileUseCase useCase)
    {
        var result = await useCase.Execute();
        
        return Ok(result);
    }
        
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [AuthenticatedUser]
    public async Task<IActionResult> Update([FromServices] IUpdateUserUseCase useCase,
        [FromBody] RequestUpdateUserJson request)
    {
        await useCase.Execute(request);
            
        return NoContent();
    }
        
    [HttpPut("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [AuthenticatedUser]
    public async Task<IActionResult> ChangePassword([FromServices] IChangePasswordUseCase useCase,
        [FromBody] RequestChangePasswordJson request)
    {
        var result = await useCase.Execute(request);
            
        return Ok(result);
    }
        
    [HttpDelete("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AuthenticatedUser]
    public async Task<IActionResult> SoftDelete([FromServices] ISoftDeleteUserUseCase userUseCase)
    {
        await userUseCase.Execute();
            
        return NoContent();
    }
        
}