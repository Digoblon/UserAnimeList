using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.Anime;

namespace UserAnimeList.Application.UseCases.Anime.Search;

public class SearchAnimeUseCase : ISearchAnimeUseCase
{
    private readonly IAnimeRepository _animeRepository;
    private readonly IAppMapper _mapper;
    
    public SearchAnimeUseCase(IAnimeRepository animeRepository, 
        IAppMapper mapper)
    {
        _animeRepository = animeRepository;
        _mapper = mapper;
    }
    
    public async Task<ResponseAnimesJson> Execute(RequestAnimeSearchJson request)
    {

        if(!Validate(request))
            return new ResponseAnimesJson();
        
        var animes = await _animeRepository.Search(request.Query);

        var animesList = _mapper.Map<IList<ResponseShortAnimeJson>>(animes);

        foreach (var anime in animesList)
        {
            var score = await _animeRepository.AverageScore(anime.Id);

            if(score is not null)
                score = Math.Round(score.Value, 2, MidpointRounding.AwayFromZero);
            
            anime.Score = score;
        }

        return new ResponseAnimesJson
        {
            Animes = animesList
        };
    }

    private static bool Validate(RequestAnimeSearchJson request)
    {
        if(request.Query.Equals("*"))
            return true;
        
        return !string.IsNullOrWhiteSpace(request.Query) && request.Query.Length >= 3;
    }
}