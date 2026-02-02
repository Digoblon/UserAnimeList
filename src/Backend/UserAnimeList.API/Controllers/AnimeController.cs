using Microsoft.AspNetCore.Mvc;
using UserAnimeList.Application.UseCases.Anime.Delete.SoftDelete;
using UserAnimeList.Application.UseCases.Anime.Get.ById;
using UserAnimeList.Application.UseCases.Anime.Register;
using UserAnimeList.Application.UseCases.Anime.Search;
using UserAnimeList.Application.UseCases.Anime.Update;
using UserAnimeList.Attributes;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Controllers;

public class AnimeController : UserAnimeListBaseController
{
    
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [AdminOnly]
        public async Task<IActionResult> Register(
            [FromServices]IRegisterAnimeUseCase useCase,
            [FromBody]RequestAnimeJson request)
        {
            var result = await useCase.Execute(request);
            
            return Created(string.Empty, result);
        }
        
        
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
        
        
        [HttpPost("search")]
        [ProducesResponseType(typeof(ResponseAnimesJson), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Search([FromServices] ISearchAnimeUseCase useCase,
            [FromBody]RequestAnimeSearchJson request)
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