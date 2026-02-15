using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Anime.Filter;

public class FilterAnimeUseCase : IFilterAnimeUseCase
{
    private readonly IAnimeRepository _animeRepository;
    private readonly IAppMapper _mapper;
    
    public FilterAnimeUseCase(IAnimeRepository animeRepository, 
        IAppMapper mapper)
    {
        _animeRepository = animeRepository;
        _mapper = mapper;
    }
    
    public async Task<ResponseAnimesJson> Execute(RequestAnimeFilterJson request)
    {
        Validate(request);

        var animes = await _animeRepository.Filter(request);

        var dtos = _mapper.Map<IList<ResponseShortAnimeJson>>(animes);

        return new ResponseAnimesJson
        {
            Animes = dtos
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