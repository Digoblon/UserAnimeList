using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.AnimeList;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.AnimeList.List.ByUserId;

public class ListAnimeByUserIdUseCase : IListAnimeByUserIdUseCase
{
    private readonly IAppMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IAnimeListRepository _animeListRepository;

    public ListAnimeByUserIdUseCase(IAppMapper mapper,
        IUserRepository userRepository,
        IAnimeListRepository animeListRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _animeListRepository = animeListRepository;
    }
    
    public async Task<ResponseAnimeListsJson> Execute(string id, RequestAnimeListEntryFilterJson request)
    {
        Validate(request);
        
        if(!Guid.TryParse(id, out var userId))
            throw new InvalidIdException(ResourceMessagesException.INVALID_ID);
        
        var user = await _userRepository.GetById(userId);

        if (user is null || !user.IsActive)
            throw new NotFoundException(ResourceMessagesException.USER_NOT_FOUND);

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