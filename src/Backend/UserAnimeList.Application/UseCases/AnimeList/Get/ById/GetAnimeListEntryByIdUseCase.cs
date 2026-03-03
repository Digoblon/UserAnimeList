using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.AnimeList;
using UserAnimeList.Domain.Services.LoggedUser;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.AnimeList.Get.ById;
 
public class GetAnimeListEntryByIdUseCase : IGetAnimeListEntryByIdUseCase
{
    private readonly IAppMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    private readonly IAnimeListRepository _animeListRepository;
    
    public GetAnimeListEntryByIdUseCase(IAppMapper mapper, 
        ILoggedUser loggedUser,
        IAnimeListRepository animeListRepository)
    {
        _mapper = mapper;
        _loggedUser = loggedUser;
        _animeListRepository = animeListRepository;
    }
    
    public async Task<ResponseAnimeListEntryJson> Execute(string id)
    {
        var loggedUser = await _loggedUser.User();

        var animeList = await _animeListRepository.GetById(id, loggedUser.Id);

        if (animeList is null)
            throw new NotFoundException(ResourceMessagesException.ANIME_LIST_INVALID);
        
        var response = _mapper.Map<ResponseAnimeListEntryJson>(animeList);
        return response;
    }
}