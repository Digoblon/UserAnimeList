using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Token;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.Token.RefreshToken;

public class UseRefreshTokenUseCase : IUseRefreshTokenUseCase
{
    
    private readonly ITokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IRefreshTokenValidator _refreshTokenValidator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    
    public UseRefreshTokenUseCase(ITokenRepository tokenRepository,
        IUnitOfWork unitOfWork,
        IAccessTokenGenerator accessTokenGenerator,
        IRefreshTokenValidator refreshTokenValidator,
        IRefreshTokenGenerator refreshTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _tokenRepository = tokenRepository;
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenValidator = refreshTokenValidator;
        _refreshTokenGenerator = refreshTokenGenerator;
    }
    
    public async Task<ResponseTokensJson> Execute(RequestNewTokenJson request)
    {
        var refreshToken = await _tokenRepository.Get(request.RefreshToken);

        _refreshTokenValidator.Validate(refreshToken);
        
        if (!refreshToken!.User.IsActive)
            throw new UserHasNoAccessException();
        
        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            Token = _refreshTokenGenerator.Generate(),
            UserId = refreshToken.UserId
        };
        
        refreshToken.RevokedOn = DateTime.UtcNow;
        _tokenRepository.RevokeRefreshToken(refreshToken);
        
        await _tokenRepository.SaveNewRefreshToken(newRefreshToken);

        await _unitOfWork.Commit();

        return new ResponseTokensJson
        {
            AccessToken = _accessTokenGenerator.Generate(refreshToken.User.Id, refreshToken.User.TokenVersion,refreshToken.User.Role),
            RefreshToken = newRefreshToken.Token
        };
    }
}