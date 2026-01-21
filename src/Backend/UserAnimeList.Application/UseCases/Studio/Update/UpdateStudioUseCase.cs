using FluentValidation.Results;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Studio;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Studio.Update;

public class UpdateStudioUseCase : IUpdateStudioUseCase
{
    private readonly IStudioRepository _studioRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateStudioUseCase(IStudioRepository repository, 
        IUnitOfWork unitOfWork)
    {
        _studioRepository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute(RequestUpdateStudioJson request, string id)
    {
        var studio = await _studioRepository.GetById(id);
        
        if (studio is null)
                    throw new NotFoundException(ResourceMessagesException.STUDIO_NOT_FOUND);
        
        await Validate(request, studio.NameNormalized);
        
        studio.Name = request.Name;
        studio.NameNormalized = studio.Name.ToLower();
        studio.Description = request.Description;
        
        _studioRepository.Update(studio);
        
        await _unitOfWork.Commit();

    }

    private async Task Validate(RequestUpdateStudioJson request, string currentName)
    {
        var validator = new UpdateStudioValidator();
        
        var result = await validator.ValidateAsync(request);
        
        if(!request.Name.ToLower().Equals(currentName))
        {
            var newNameExist = await _studioRepository.ExistsActiveStudioWithName(request.Name);

            if (newNameExist)
                result.Errors.Add(new ValidationFailure(string.Empty,
                    ResourceMessagesException.STUDIO_ALREADY_REGISTERED));
        }
        
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}