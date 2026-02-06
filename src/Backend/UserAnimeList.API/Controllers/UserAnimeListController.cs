using Microsoft.AspNetCore.Mvc;
using UserAnimeList.Application.UseCases.UserAnimeList.AddEntry;
using UserAnimeList.Application.UseCases.UserAnimeList.Delete;
using UserAnimeList.Application.UseCases.UserAnimeList.Get.ById;
using UserAnimeList.Application.UseCases.UserAnimeList.List;
using UserAnimeList.Application.UseCases.UserAnimeList.List.Me;
using UserAnimeList.Application.UseCases.UserAnimeList.Update;
using UserAnimeList.Attributes;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Controllers;

public class UserAnimeListController : UserAnimeListBaseController
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [AuthenticatedUser]
    public async Task<IActionResult> AddEntry(
        [FromServices]IAddAnimeListEntryUseCase useCase,
        [FromBody]RequestAnimeListEntryJson request)
    {
        var result = await useCase.Execute(request);
        
        return Created(string.Empty, result);
    }
    
    
    [HttpGet]
    [Route("{id}")]
    [AuthenticatedUser]
    [ProducesResponseType(typeof(ResponseAnimeJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAnimeById([FromServices] IGetAnimeListEntryByIdUseCase useCase,
        [FromRoute]string id)
    {
        var result = await useCase.Execute(id);
        
        return Ok(result);
    }
    
    /*
    [HttpPost("me/filter")]
    [ProducesResponseType(typeof(ResponseAnimeListsJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Search([FromServices] IFilterAnimeListEntryUseCase useCase,
        [FromBody]RequestAnimeListEntryFilterJson request)
    {
        var response = await useCase.Execute(request);
        if(response.Lists.Any())   
            return Ok(response);

        return NoContent();
    }
    
    [HttpPost("filter/{userId}")]
    [ProducesResponseType(typeof(ResponseAnimeListsJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Search([FromServices] IFilterAnimeListEntryByUserIdUseCase useCase,
        [FromBody]RequestAnimeListEntryFilterJson request,
        [FromRoute]string userId)
    {
        var response = await useCase.Execute(request,userId);
        if(response.Lists.Any())   
            return Ok(response);

        return NoContent();
    }
    */
    
    [HttpGet("list/{userId}")]
    [ProducesResponseType(typeof(ResponseAnimeListsJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Search([FromServices] IListAnimeByUserIdUseCase useCase,
        [FromRoute]string userId,
        [FromQuery]RequestAnimeListEntryFilterJson request)
    {
        var response = await useCase.Execute(userId, request);
        
        return Ok(response.Lists.Any() ? response : new ResponseAnimeListsJson());
    }
    
    [HttpGet("me/list")]
    [ProducesResponseType(typeof(ResponseAnimeListsJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AuthenticatedUser]
    public async Task<IActionResult> Search([FromServices] IListAnimeUseCase useCase,
        [FromQuery]RequestAnimeListEntryFilterJson request)
    {
        var response = await useCase.Execute(request);
        
        return Ok(response.Lists.Any() ? response : new ResponseAnimeListsJson());
    }
    
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [AuthenticatedUser]
    public async Task<IActionResult> Update([FromServices] IUpdateAnimeListEntryUseCase useCase,
        [FromBody] RequestUpdateAnimeListEntryJson request,
        [FromRoute]string id)
    {
        await useCase.Execute(request, id);
        
        return NoContent();
    }

    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AuthenticatedUser]
    public async Task<IActionResult> SoftDelete([FromServices] IDeleteAnimeListEntryUseCase userUseCase,
        [FromRoute]string id)
    {
        await userUseCase.Execute(id);
        
        return NoContent();
    }
    
}