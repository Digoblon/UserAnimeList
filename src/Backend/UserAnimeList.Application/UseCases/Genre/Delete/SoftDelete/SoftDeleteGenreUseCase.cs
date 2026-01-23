using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Genre;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Genre.Delete.SoftDelete;

public class SoftDeleteGenreUseCase : ISoftDeleteGenreUseCase
{
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public SoftDeleteGenreUseCase(IGenreRepository genreRepository, 
        IUnitOfWork unitOfWork)
    {
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute(string id)
    {
        var genre = await _genreRepository.GetById(id);
        
        if (genre is null)
            throw new NotFoundException(ResourceMessagesException.STUDIO_NOT_FOUND);
        
        if (!genre.IsActive || genre.DeletedOn is not null)
            return;
        
        genre.DeletedOn = DateTime.UtcNow;
        genre.IsActive = false;
        
        _genreRepository.Update(genre);
        await _unitOfWork.Commit();
        
        
    }
}