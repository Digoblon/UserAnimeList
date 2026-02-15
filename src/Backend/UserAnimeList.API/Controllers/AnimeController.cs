using Microsoft.AspNetCore.Mvc;
using UserAnimeList.Application.UseCases.Anime.Delete.SoftDelete;
using UserAnimeList.Application.UseCases.Anime.Filter;
using UserAnimeList.Application.UseCases.Anime.Get.ById;
using UserAnimeList.Application.UseCases.Anime.Image.Delete;
using UserAnimeList.Application.UseCases.Anime.Image.Update;
using UserAnimeList.Application.UseCases.Anime.Register;
using UserAnimeList.Application.UseCases.Anime.Search;
using UserAnimeList.Application.UseCases.Anime.Update;
using UserAnimeList.Attributes;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Filters;

namespace UserAnimeList.Controllers;

public class AnimeController : UserAnimeListBaseController
{
    
        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisteredAnimeJson),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [AdminOnly]
        public async Task<IActionResult> Register(
            [FromServices]IRegisterAnimeUseCase useCase,
            [FromBody]RequestAnimeJson request)
        {
            var result = await useCase.Execute(request);
            
            return Created(string.Empty, result);
        }
        
        [ServiceFilter(typeof(AbsoluteImageUrlFilter))]
        [HttpPut("{id}/image")]
        [ProducesResponseType(typeof(ResponseUpdateImageJson),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [AdminOnly]
        public async Task<IActionResult> UpdateImage(
            [FromServices]IUpdateAnimeImageUseCase useCase,
            [FromForm]RequestUpdateImageFormData request,
            [FromRoute]string id)
        {
            var response = await useCase.Execute(request, id);
            
            return Ok(response);
        }
        
        [ServiceFilter(typeof(AbsoluteImageUrlFilter))]
        [HttpDelete("{id}/image")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [AdminOnly]
        public async Task<IActionResult> DeleteImage(
            [FromServices]IDeleteAnimeImageUseCase useCase,
            [FromRoute]string id)
        {
            await useCase.Execute(id);
            
            return NoContent();
        }
        
        
        [ServiceFilter(typeof(AbsoluteImageUrlFilter))]
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseAnimeJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAnimeById([FromServices] IGetAnimeByIdUseCase useCase,
            [FromRoute]string id)
        {
            var result = await useCase.Execute(id);
            
            return Ok(result);
        }
        
        [ServiceFilter(typeof(AbsoluteImageUrlFilter))]
        [HttpGet("search")]
        [ProducesResponseType(typeof(ResponseAnimesJson), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Search([FromServices] ISearchAnimeUseCase useCase,
            [FromQuery]RequestAnimeSearchJson request)
        {
            var response = await useCase.Execute(request);
            if(response.Animes.Any())   
                return Ok(response);

            return NoContent();
        }
        
        [ServiceFilter(typeof(AbsoluteImageUrlFilter))]
        [HttpGet("filter")]
        [ProducesResponseType(typeof(ResponseAnimesJson), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Filter([FromServices] IFilterAnimeUseCase useCase,
            [FromQuery]RequestAnimeFilterJson request)
        {
            var response = await useCase.Execute(request);
            if(response.Animes.Any())   
                return Ok(response);

            return NoContent();
        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [AdminOnly]
        public async Task<IActionResult> Update([FromServices] IUpdateAnimeUseCase useCase,
            [FromBody] RequestAnimeJson request,
            [FromRoute]string id)
        {
            await useCase.Execute(request, id);
            
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [AdminOnly]
        public async Task<IActionResult> SoftDelete([FromServices] ISoftDeleteAnimeUseCase userUseCase,
            [FromRoute]string id)
        {
            await userUseCase.Execute(id);
            
            return NoContent();
        }
}
