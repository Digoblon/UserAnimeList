using Microsoft.AspNetCore.Mvc;
using UserAnimeList.Application.UseCases.AnimeList.AddEntry;
using UserAnimeList.Application.UseCases.AnimeList.Delete;
using UserAnimeList.Application.UseCases.AnimeList.Get.ById;
using UserAnimeList.Application.UseCases.AnimeList.List.ByUserId;
using UserAnimeList.Application.UseCases.AnimeList.List.Me;
using UserAnimeList.Application.UseCases.AnimeList.Update;
using UserAnimeList.Attributes;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Controllers;

public class AnimeListController : UserAnimeListBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseAnimeListEntryJson),StatusCodes.Status201Created)]
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
    [ProducesResponseType(typeof(ResponseAnimeListEntryJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAnimeListEntryById([FromServices] IGetAnimeListEntryByIdUseCase useCase,
        [FromRoute]string id)
    {
        var result = await useCase.Execute(id);
        
        return Ok(result);
    }
    
    [HttpGet("list/{userId}")]
    [ProducesResponseType(typeof(ResponseAnimeListsJson), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromServices] IListAnimeByUserIdUseCase useCase,
        [FromRoute]string userId,
        [FromQuery]RequestAnimeListEntryFilterJson request)
    {
        var response = await useCase.Execute(userId, request);
        
        return Ok(response.Lists.Any() ? response : new ResponseAnimeListsJson());
    }
    
    [HttpGet("me/list")]
    [ProducesResponseType(typeof(ResponseAnimeListsJson), StatusCodes.Status200OK)]
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
    public async Task<IActionResult> Delete([FromServices] IDeleteAnimeListEntryUseCase userUseCase,
        [FromRoute]string id)
    {
        await userUseCase.Execute(id);
        
        return NoContent();
    }
    
}