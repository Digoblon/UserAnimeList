using Microsoft.AspNetCore.Mvc;
using UserAnimeList.Application.UseCases.Studio.Delete.SoftDelete;
using UserAnimeList.Application.UseCases.Studio.Get.ById;
using UserAnimeList.Application.UseCases.Studio.Get.ByName;
using UserAnimeList.Application.UseCases.Studio.Register;
using UserAnimeList.Application.UseCases.Studio.Update;
using UserAnimeList.Attributes;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Controllers;

public class StudioController : UserAnimeListBaseController
{
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisteredStudioJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status403Forbidden)]
        [AdminOnly]
        public async Task<IActionResult> Register(
            [FromServices]IRegisterStudioUseCase useCase,
            [FromBody]RequestRegisterStudioJson request)
        {
            var result = await useCase.Execute(request);
            
            return Created(string.Empty, result);
        }
        
        
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseStudioJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudioById([FromServices] IGetStudioByIdUseCase useCase,
            [FromRoute]string id)
        {
            var result = await useCase.Execute(id);
            
            return Ok(result);
        }
        
        [HttpPost("search")]
        [ProducesResponseType(typeof(ResponseStudiosJson), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStudioByName([FromServices] IGetStudioByNameUseCase useCase,
            [FromBody]RequestStudioGetByNameJson request)
        {
            var response = await useCase.Execute(request);
            if(response.Studios.Any())   
                return Ok(response);

            return NoContent();
        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        [AdminOnly]
        public async Task<IActionResult> Update([FromServices] IUpdateStudioUseCase useCase,
            [FromBody] RequestUpdateStudioJson request,
            [FromRoute]string id)
        {
            await useCase.Execute(request, id);
            
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        [AdminOnly]
        public async Task<IActionResult> SoftDelete([FromServices] ISoftDeleteStudioUseCase userUseCase,
            [FromRoute]string id)
        {
            await userUseCase.Execute(id);
            
            return NoContent();
        }
}
