using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.UserAnimeList;
using UserAnimeList.Domain.Services.LoggedUser;

namespace UserAnimeList.Application.UseCases.AnimeList.List.Me;

public class ListAnimeUseCase : IListAnimeUseCase
{
    private readonly IAppMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    private readonly IUserAnimeListRepository _animeListRepository;

    public ListAnimeUseCase(IAppMapper mapper,
        ILoggedUser loggedUser,
        IUserAnimeListRepository animeListRepository)
    {
        _mapper = mapper;
        _loggedUser = loggedUser;
        _animeListRepository = animeListRepository;
    }
    
    public async Task<ResponseAnimeListsJson> Execute(RequestAnimeListEntryFilterJson request)
    {
        var user = await _loggedUser.User();

        var animeLists = await _animeListRepository.List(user.Id, request);

        var animeListsDto = _mapper.Map<IList<ResponseShortAnimeListEntryJson>>(animeLists);

        return new ResponseAnimeListsJson
        {
            Lists = animeListsDto
        };
    }
}