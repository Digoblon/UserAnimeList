using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.Genre;

namespace UserAnimeList.Application.UseCases.Genre.Get.ByName;

public class GetGenreByNameUseCase :  IGetGenreByNameUseCase
{
    private readonly IGenreRepository _repository;
    private readonly IAppMapper _mapper;
    public GetGenreByNameUseCase(IGenreRepository repository,
        IAppMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ResponseGenresJson> Execute(RequestGenreGetByNameJson request)
    {
        var genres = await _repository.SearchByName(request.Name);
        
        var genresDto = _mapper.Map<IList<ResponseGenreJson>>(genres);

        return new ResponseGenresJson
        {
            Genres = genresDto
        };
    }
}