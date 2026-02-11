using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Anime.Get.ById;

public class GetAnimeByIdUseCase : IGetAnimeByIdUseCase
{
    private readonly IAppMapper _mapper;
    private readonly IAnimeRepository _animeRepository;
    
    public GetAnimeByIdUseCase(IAppMapper mapper, 
        IAnimeRepository animeRepository)
    {
        _mapper = mapper;
        _animeRepository = animeRepository;
    }
    
    public async Task<ResponseAnimeJson> Execute(string id)
    {
        var anime = await _animeRepository.GetById(id);

        if (anime is null)
            throw new NotFoundException(ResourceMessagesException.ANIME_NOT_FOUND);
        
        var score = await _animeRepository.AverageScore(anime.Id);

        if(score is not null)
            score = Math.Round(score.Value, 2, MidpointRounding.AwayFromZero);
        
        var response = _mapper.Map<ResponseAnimeJson>(anime);
        response.Genres = anime.Genres.Select(g => g.GenreId).ToList();
        response.Studios = anime.Studios.Select(s => s.StudioId).ToList();
        response.Score = score;
        
        if(anime.Premiered.HasValue)
        {
            response.Premiered = $"{anime.Premiered.Value.Season} {anime.Premiered.Value.Year}";
        }

        return response;
    }
}