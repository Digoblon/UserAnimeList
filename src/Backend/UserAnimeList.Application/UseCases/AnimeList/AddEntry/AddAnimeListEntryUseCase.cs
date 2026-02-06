using FluentValidation.Results;
using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Domain.Repositories.UserAnimeList;
using UserAnimeList.Domain.Services.LoggedUser;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.AnimeList.AddEntry;

public class AddAnimeListEntryUseCase : IAddAnimeListEntryUseCase
{
    private readonly IAppMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    private readonly IAnimeRepository _animeRepository;
    private readonly IUserAnimeListRepository _animeListRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public AddAnimeListEntryUseCase(IAppMapper mapper,
        ILoggedUser loggedUser,
        IAnimeRepository animeRepository,
        IUserAnimeListRepository animeListRepository,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _loggedUser = loggedUser;
        _animeRepository = animeRepository;
        _animeListRepository = animeListRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ResponseAnimeListEntryJson> Execute(RequestAnimeListEntryJson request)
    {
        var user = await _loggedUser.User();
        var anime = await _animeRepository.GetById(request.AnimeId.ToString());

        if (anime is null)
            throw new NotFoundException(ResourceMessagesException.ANIME_NOT_FOUND);
        
        await Validate(request, user, anime);
        
        var animeList = _mapper.Map<Domain.Entities.AnimeList>(request);
        animeList.UserId = user.Id;
        animeList.AnimeId = anime.Id;

        switch (request.Status)
        {
            case AnimeEntryStatus.Watching:
                if (!request.DateStarted.HasValue)
                    animeList.DateStarted = DateOnly.FromDateTime(DateTime.UtcNow);
                break;
            
            case AnimeEntryStatus.Completed:
                if (!request.DateStarted.HasValue)
                    animeList.DateStarted = DateOnly.FromDateTime(DateTime.UtcNow);
                if(!request.DateFinished.HasValue)
                    animeList.DateFinished = DateOnly.FromDateTime(DateTime.UtcNow);
                if (!request.Progress.HasValue)
                    animeList.Progress = anime.Episodes;
                break;
        }
        
        await _animeListRepository.Add(animeList);
        
        await _unitOfWork.Commit();
        
        return _mapper.Map<ResponseAnimeListEntryJson>(animeList);
    }

    private async Task Validate(RequestAnimeListEntryJson request,Domain.Entities.User user, Domain.Entities.Anime anime)
    {
        var validator = new AnimeListValidator();
        var result = await validator.ValidateAsync(request);
        
        var entryExists = await _animeListRepository.ExistsEntry(user.Id, anime.Id);
        if (entryExists)
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.ANIME_LIST_ENTRY_ALREADY_EXISTS));

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