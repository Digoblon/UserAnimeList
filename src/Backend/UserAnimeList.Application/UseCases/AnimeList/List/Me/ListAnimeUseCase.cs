using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.AnimeList;
using UserAnimeList.Domain.Services.LoggedUser;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.AnimeList.List.Me;

public class ListAnimeUseCase : IListAnimeUseCase
{
    private readonly IAppMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    private readonly IAnimeListRepository _animeListRepository;

    public ListAnimeUseCase(IAppMapper mapper,
        ILoggedUser loggedUser,
        IAnimeListRepository animeListRepository)
    {
        _mapper = mapper;
        _loggedUser = loggedUser;
        _animeListRepository = animeListRepository;
    }
    
    public async Task<ResponseAnimeListsJson> Execute(RequestAnimeListEntryFilterJson request)
    {
        Validate(request);
        
        var user = await _loggedUser.User();

        var animeLists = await _animeListRepository.List(user.Id, request);

        var animeListsDto = _mapper.Map<IList<ResponseShortAnimeListEntryJson>>(animeLists);

        return new ResponseAnimeListsJson
        {
            Lists = animeListsDto
        };
    }

    private static void Validate(RequestAnimeListEntryFilterJson request)
    {
        var validator = new AnimeListFilterValidator();
        var result = validator.Validate(request);
        
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).Distinct().ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}