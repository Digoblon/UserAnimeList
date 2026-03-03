using FluentValidation.Results;
using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Genre;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Genre.Register;

public class RegisterGenreUseCase : IRegisterGenreUseCase
{
    private readonly IAppMapper _mapper;
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public RegisterGenreUseCase(IAppMapper mapper,
        IGenreRepository genreRepository,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _genreRepository = genreRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ResponseRegisteredGenreJson> Execute(RequestRegisterGenreJson request)
    {
        await Validate(request);

        var genre = _mapper.Map<Domain.Entities.Genre>(request);
        genre.NameNormalized = request.Name.ToLower();
        
        await _genreRepository.Add(genre);
        
        await _unitOfWork.Commit();

        return new ResponseRegisteredGenreJson
        {
            Name = genre.Name,
            Description = genre.Description
        };
    }

    private async Task Validate(RequestRegisterGenreJson request)
    {
        var validator = new RegisterGenreValidator();
        
        var result = await validator.ValidateAsync(request);
        
        var nameExist = await _genreRepository.ExistsActiveGenreWithName(request.Name);
        if(nameExist)
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.GENRE_ALREADY_REGISTERED));
        
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}