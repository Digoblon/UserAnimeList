using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Anime.Delete.SoftDelete;

public class SoftDeleteAnimeUseCase : ISoftDeleteAnimeUseCase
{
    private readonly IAnimeRepository _animeRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public SoftDeleteAnimeUseCase(IAnimeRepository animeRepository,
        IUnitOfWork unitOfWork)
    {
        _animeRepository = animeRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute(string id)
    {
        var anime = await _animeRepository.GetById(id);

        if (anime is null)
            throw new NotFoundException(ResourceMessagesException.ANIME_NOT_FOUND);
        
        if(!anime.IsActive || anime.DeletedOn is not null)
            return;

        anime.IsActive = false;
        anime.DeletedOn = DateTime.UtcNow;
        
        _animeRepository.Update(anime);
        await _unitOfWork.Commit();
    }
}