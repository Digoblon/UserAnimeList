using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.UserAnimeList;
using UserAnimeList.Domain.Services.LoggedUser;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.AnimeList.Delete;

public class DeleteAnimeListEntryUseCase : IDeleteAnimeListEntryUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserAnimeListRepository _animeListRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteAnimeListEntryUseCase(ILoggedUser loggedUser,
    IUserAnimeListRepository animeListRepository,
        IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _animeListRepository = animeListRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute(string id)
    {
        var loggedUser = await _loggedUser.User();

        var animeList = await _animeListRepository.GetById(id, loggedUser.Id);

        if (animeList is null)
            throw new NotFoundException(ResourceMessagesException.ANIME_LIST_INVALID);
        
        _animeListRepository.Delete(animeList);
        
        await _unitOfWork.Commit();
    }
}