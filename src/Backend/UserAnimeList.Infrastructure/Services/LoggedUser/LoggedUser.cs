using UserAnimeList.Domain.Entities;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Domain.Services.LoggedUser;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Infrastructure.Services.LoggedUser;

public class LoggedUser : ILoggedUser
{
    private readonly ITokenProvider _tokenProvider;
    private readonly IUserRepository _repository;

    public LoggedUser(IUserRepository repository,
        ITokenProvider tokenProvider)
    {
        _repository = repository;
        _tokenProvider = tokenProvider;
    }

    public async Task<User> User()
    {
        var tokenData = _tokenProvider.Value();

        var user = await _repository.GetById(tokenData.UserId)
                   ?? throw new UnauthorizedException(ResourceMessagesException.USER_NOT_FOUND);

        return user;
            
    }
}