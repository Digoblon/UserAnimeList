using FileTypeChecker.Exceptions;
using UserAnimeList.Application.Extensions;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Domain.Services.FileStorage;
using UserAnimeList.Domain.Services.LoggedUser;
using UserAnimeList.Domain.ValueObjects;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Anime.Image.Update;

public class UpdateAnimeImageUseCase : IUpdateAnimeImageUseCase
{
    private readonly IAnimeRepository _animeRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateAnimeImageUseCase(IAnimeRepository animeRepository,
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork)
    {
        _animeRepository = animeRepository;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ResponseUpdateImageJson> Execute(RequestUpdateImageFormData request, string id)
    {
        var fileStream = ValidateAndGetStream(request);

        var anime = await _animeRepository.GetById(id);
        if (anime is null)
            throw new NotFoundException(ResourceMessagesException.ANIME_NOT_FOUND);

        var fileExtension = fileStream.GetImageExtension();
        
        if(!string.IsNullOrWhiteSpace(anime.ImagePath))
            _fileStorage.Delete(anime.ImagePath);
        
        var imagemPath = await _fileStorage.Save(anime.GetType(),anime.Id,fileStream, fileExtension);

        anime.ImagePath = imagemPath;

        _animeRepository.Update(anime);

        await _unitOfWork.Commit();

        return new ResponseUpdateImageJson
        {
            ImageUrl = imagemPath.Replace('\\', '/')
        };
    }
    
    private static Stream ValidateAndGetStream(RequestUpdateImageFormData request)
    {
        if (request.Image is null || request.Image.Length == 0)
            throw new ErrorOnValidationException([ResourceMessagesException.IMAGE_NULL]);

        if (request.Image.Length > UserAnimeListConstants.MaxImageSize)
            throw new ErrorOnValidationException([ResourceMessagesException.IMAGE_EXCEEDS_FILE_SIZE]);
        
        var fileStream = request.Image.OpenReadStream();

        try
        {
            if (!fileStream.Validate())
                throw new ErrorOnValidationException([ResourceMessagesException.ONLY_IMAGES_ACCEPTED]);
        }
        catch (TypeNotFoundException)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.ONLY_IMAGES_ACCEPTED]);
        }
        

        return fileStream;
    }
}