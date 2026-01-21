using FluentValidation.Results;
using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Studio;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Studio.Register;

public class RegisterStudioUseCase : IRegisterStudioUseCase
{
    private readonly IAppMapper _mapper;
    private readonly IStudioRepository _studioRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RegisterStudioUseCase(IAppMapper mapper,
        IStudioRepository studioRepository,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _studioRepository = studioRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ResponseRegisteredStudioJson> Execute(RequestRegisterStudioJson request)
    {
        await Validate(request);

        var studio = _mapper.Map<Domain.Entities.Studio>(request);
        studio.NameNormalized = request.Name.ToLower();
        
        await _studioRepository.Add(studio);
        
        await _unitOfWork.Commit();

        return new ResponseRegisteredStudioJson
        {
            Name = studio.Name,
            Description = studio.Description
        };
    }

    private async Task Validate(RequestRegisterStudioJson request)
    {
        var validator = new RegisterStudioValidator();
        
        var result = await validator.ValidateAsync(request);
        
        var nameExist = await _studioRepository.ExistsActiveStudioWithName(request.Name);
        if(nameExist)
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.STUDIO_ALREADY_REGISTERED));
        
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}