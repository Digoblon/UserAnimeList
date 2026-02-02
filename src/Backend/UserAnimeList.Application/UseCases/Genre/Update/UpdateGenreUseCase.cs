using FluentValidation.Results;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Genre;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Genre.Update;

public class UpdateGenreUseCase : IUpdateGenreUseCase
{
    private readonly IGenreRepository _genreRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateGenreUseCase(IGenreRepository repository, 
        IUnitOfWork unitOfWork)
    {
        _genreRepository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute(RequestUpdateGenreJson request, string id)
    {
        var genre = await _genreRepository.GetById(id);
        
        if (genre is null)
                    throw new NotFoundException(ResourceMessagesException.GENRE_NOT_FOUND);
        
        await Validate(request, genre.NameNormalized);
        
        genre.Name = request.Name;
        genre.NameNormalized = genre.Name.ToLower();
        genre.Description = request.Description;
        
        _genreRepository.Update(genre);
        
        await _unitOfWork.Commit();

    }

    private async Task Validate(RequestUpdateGenreJson request, string currentName)
    {
        var validator = new UpdateGenreValidator();
        
        var result = await validator.ValidateAsync(request);
        
        if(!request.Name.ToLower().Equals(currentName))
        {
            var newNameExist = await _genreRepository.ExistsActiveGenreWithName(request.Name);

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