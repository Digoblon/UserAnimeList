using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.Genre;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Genre.Get.ById;

public class GetGenreByIdUseCase : IGetGenreByIdUseCase
{
    private readonly IGenreRepository _repository;
    private readonly IAppMapper _mapper;
    public GetGenreByIdUseCase(IGenreRepository repository,
        IAppMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<ResponseGenreJson> Execute(string id)
    {
        var genre = await _repository.GetById(id);

        if (genre is null || !genre.IsActive)
            throw new NotFoundException(ResourceMessagesException.GENRE_NOT_FOUND);
        
        var response = _mapper.Map<ResponseGenreJson>(genre);
        
        return response;
    }
}