using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Anime.Filter;

public class FilterAnimeUseCase : IFilterAnimeUseCase
{
    private readonly IAnimeRepository _animeRepository;
    
    public FilterAnimeUseCase(IAnimeRepository animeRepository)
    {
        _animeRepository = animeRepository;
    }
    
    public async Task<ResponseAnimesJson> Execute(RequestAnimeFilterJson request)
    {
        Validate(request);

        var animes = await _animeRepository.FilterWithScore(request);

        foreach (var anime in animes)
        {
            anime.Score = anime.Score is null
                ? null
                : Math.Round(anime.Score.Value, 2, MidpointRounding.AwayFromZero);
        }
        
        return new ResponseAnimesJson
        {
            Animes = animes
        };
    }

    private static void Validate(RequestAnimeFilterJson request)
    {
        var validator = new AnimeFilterValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}