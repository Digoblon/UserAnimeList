using FluentValidation.Results;
using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Domain.Repositories.UserAnimeList;
using UserAnimeList.Domain.Services.LoggedUser;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.AnimeList.Update;

public class UpdateAnimeListEntryUseCase : IUpdateAnimeListEntryUseCase
{
    private readonly IAppMapper _mapper;
    private readonly IUserAnimeListRepository _animeListRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateAnimeListEntryUseCase(IAppMapper mapper,
        IUserAnimeListRepository userAnimeListRepository,
        ILoggedUser loggedUser,
        IAnimeRepository animeRepository,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _animeListRepository = userAnimeListRepository;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }
    public async Task Execute(RequestUpdateAnimeListEntryJson request, string id)
    {
        var loggedUser = await _loggedUser.User();
        
        var animeList = await _animeListRepository.GetById(id, loggedUser.Id);
        

        if (animeList is null)
            throw new NotFoundException(ResourceMessagesException.ANIME_LIST_INVALID);
        
        var animeListId  = animeList.Id;
        
        var anime = animeList.Anime;
        
        await Validate(request, anime);

        animeList = _mapper.Map<Domain.Entities.AnimeList>(request);

        animeList.Id = animeListId;
        animeList.AnimeId = anime.Id;
        animeList.UserId = loggedUser.Id;

        _animeListRepository.Update(animeList);
        
        await _unitOfWork.Commit();

    }

    private static async Task Validate(RequestUpdateAnimeListEntryJson request, Domain.Entities.Anime anime)
    {
        var validator = new UpdateAnimeListEntryValidator();
        var result = await validator.ValidateAsync(request);

        if(request.Progress.HasValue && anime.Episodes.HasValue)
        {
            if (request.Progress > anime.Episodes)
                result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.EPISODE_COUNT_INVALID));
        }
        
        if(!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}