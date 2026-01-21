using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Studio;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Studio.Delete.SoftDelete;

public class SoftDeleteStudioUseCase : ISoftDeleteStudioUseCase
{
    private readonly IStudioRepository _studioRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public SoftDeleteStudioUseCase(IStudioRepository studioRepository, 
        IUnitOfWork unitOfWork)
    {
        _studioRepository = studioRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute(string id)
    {
        var studio = await _studioRepository.GetById(id);
        
        if (studio is null)
            throw new NotFoundException(ResourceMessagesException.STUDIO_NOT_FOUND);
        
        if (!studio.IsActive || studio.DeletedOn is not null)
            return;
        
        studio.DeletedOn = DateTime.UtcNow;
        studio.IsActive = false;
        
        _studioRepository.Update(studio);
        await _unitOfWork.Commit();
        
        
    }
}