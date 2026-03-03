using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.Anime;

namespace UserAnimeList.Application.UseCases.Anime.Search;

public class SearchAnimeUseCase : ISearchAnimeUseCase
{
    private readonly IAnimeRepository _animeRepository;
    
    public SearchAnimeUseCase(IAnimeRepository animeRepository)
    {
        _animeRepository = animeRepository;
    }
    
    public async Task<ResponseAnimesJson> Execute(RequestAnimeSearchJson request)
    {
        if(!Validate(request.Query))
            return new ResponseAnimesJson();
            
        
        var animes = await _animeRepository.SearchWithScore(request.Query!);

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

    private static bool Validate(string? query)
    {
        return !string.IsNullOrWhiteSpace(query) && query.Length >= 3;
    }
}
