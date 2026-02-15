using FileTypeChecker.Exceptions;
using UserAnimeList.Application.Extensions;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Services.FileStorage;
using UserAnimeList.Domain.Services.LoggedUser;
using UserAnimeList.Domain.ValueObjects;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.User.Image.Update;

public class UpdateUserImageUseCase : IUpdateUserImageUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserRepository _userRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateUserImageUseCase(ILoggedUser loggedUser,
        IUserRepository userRepository,
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _userRepository = userRepository;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseUpdateImageJson> Execute(RequestUpdateImageFormData request)
    {
        var fileStream = ValidateAndGetStream(request);
        
        var user = await _loggedUser.User();

        var fileExtension = fileStream.GetImageExtension();
        
        if(!string.IsNullOrWhiteSpace(user.ImagePath))
             _fileStorage.Delete(user.ImagePath);
        
        var imagemPath = await _fileStorage.Save(user.GetType(),user.Id,fileStream, fileExtension);

        user.ImagePath = imagemPath;

        _userRepository.Update(user);

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