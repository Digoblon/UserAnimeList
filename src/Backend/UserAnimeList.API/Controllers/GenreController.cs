using Microsoft.AspNetCore.Mvc;
using UserAnimeList.Application.UseCases.Genre.Delete.SoftDelete;
using UserAnimeList.Application.UseCases.Genre.Get.ById;
using UserAnimeList.Application.UseCases.Genre.Get.ByName;
using UserAnimeList.Application.UseCases.Genre.Register;
using UserAnimeList.Application.UseCases.Genre.Update;
using UserAnimeList.Attributes;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Controllers;

public class GenreController : UserAnimeListBaseController
{
    [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [AdminOnly]
        public async Task<IActionResult> Register(
            [FromServices]IRegisterGenreUseCase useCase,
            [FromBody]RequestRegisterGenreJson request)
        {
            var result = await useCase.Execute(request);
            
            return Created(string.Empty, result);
        }
        
        
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseGenreJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGenreById([FromServices] IGetGenreByIdUseCase useCase,
            [FromRoute]string id)
        {
            var result = await useCase.Execute(id);
            
            return Ok(result);
        }
        
        [HttpPost("search")]
        [ProducesResponseType(typeof(ResponseGenresJson), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetGenreByName([FromServices] IGetGenreByNameUseCase useCase,
            [FromBody]RequestGenreGetByNameJson request)
        {
            var response = await useCase.Execute(request);
            if(response.Genres.Any())   
                return Ok(response);

            return NoContent();
        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [AdminOnly]
        public async Task<IActionResult> Update([FromServices] IUpdateGenreUseCase useCase,
            [FromBody] RequestUpdateGenreJson request,
            [FromRoute]string id)
        {
            await useCase.Execute(request, id);
            
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [AdminOnly]
        public async Task<IActionResult> SoftDelete([FromServices] ISoftDeleteGenreUseCase useCase,
            [FromRoute]string id)
        {
            await useCase.Execute(id);
            
            return NoContent();
        }
}