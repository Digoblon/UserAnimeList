using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Services.FileStorage;
using UserAnimeList.Domain.Services.LoggedUser;

namespace UserAnimeList.Application.UseCases.User.Image.Delete;

public class DeleteUserImageUseCase : IDeleteUserImageUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserRepository _userRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteUserImageUseCase(ILoggedUser loggedUser, 
        IUserRepository userRepository,
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _userRepository = userRepository;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute()
    {
        var user = await _loggedUser.User();
        
        if (string.IsNullOrWhiteSpace(user.ImagePath))
            return;
        
        _fileStorage.Delete(user.ImagePath);
        user.ImagePath = null;
        _userRepository.Update(user);
        
        await _unitOfWork.Commit();
    }
}