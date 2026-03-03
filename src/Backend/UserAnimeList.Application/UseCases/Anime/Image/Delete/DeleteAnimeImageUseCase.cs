using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Domain.Services.FileStorage;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Anime.Image.Delete;

public class DeleteAnimeImageUseCase : IDeleteAnimeImageUseCase
{
    private readonly IAnimeRepository _animeRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteAnimeImageUseCase(IAnimeRepository animeRepository, 
        IFileStorage fileStorage, 
        IUnitOfWork unitOfWork)
    {
        _animeRepository = animeRepository;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute(string id)
    {
        var anime = await _animeRepository.GetById(id);
        
        if(anime is null)
            throw new NotFoundException(ResourceMessagesException.ANIME_NOT_FOUND);


        if (anime.ImagePath is null)
            return;
        
        _fileStorage.Delete(anime.ImagePath);
        anime.ImagePath = null;
        _animeRepository.Update(anime);
        await _unitOfWork.Commit();
    }
}