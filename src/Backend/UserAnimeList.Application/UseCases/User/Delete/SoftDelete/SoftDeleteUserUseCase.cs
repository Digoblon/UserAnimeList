using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Token;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Services.LoggedUser;

namespace UserAnimeList.Application.UseCases.User.Delete.SoftDelete;

public class SoftDeleteUserUseCase : ISoftDeleteUserUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenRepository _tokenRepository;
    
    public SoftDeleteUserUseCase(ILoggedUser loggedUser, 
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork,
        ITokenRepository tokenRepository)
    {
        _loggedUser = loggedUser;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenRepository = tokenRepository;
    }
    
    public async Task Execute()
    {
        var user = await _loggedUser.User();
        
        if (!user.IsActive || user.DeletedOn is not null)
            return;
        
        _userRepository.Delete(user);
        await _tokenRepository.RevokeAllForUser(user.Id);
        
        await _unitOfWork.Commit();
        
        
    }
}