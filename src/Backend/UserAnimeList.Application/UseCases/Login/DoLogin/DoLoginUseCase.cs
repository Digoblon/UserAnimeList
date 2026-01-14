using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Security.Cryptography;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Login.DoLogin;

public class DoLoginUseCase : IDoLoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IAccessTokenGenerator _accessTokenGenerator;

    public DoLoginUseCase(IUserRepository userRepository,
        IPasswordEncrypter passwordEncrypter,
        IAccessTokenGenerator accessTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordEncrypter = passwordEncrypter;
        _accessTokenGenerator = accessTokenGenerator;
    }
    
    public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
    {
        var user = await _userRepository.GetByLogin(request.Login);
        
        if (user is null || !_passwordEncrypter.IsValid(request.Password,user.Password))
            throw new InvalidLoginException();
        

        return new ResponseRegisteredUserJson
        {
            UserName = user.UserName,
            Tokens = new ResponseTokensJson
                {   
                    AccessToken = _accessTokenGenerator.Generate(user.Id, user.TokenVersion)
                }
        };
    }
}