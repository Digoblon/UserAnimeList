using FluentValidation.Results;
using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Token;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Security.Cryptography;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.User.Register;

public class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IAppMapper _mapper;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly ITokenRepository _tokenRepository;

    public RegisterUserUseCase(IAppMapper mapper, 
        IPasswordEncrypter passwordEncrypter,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IAccessTokenGenerator accessTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        ITokenRepository tokenRepository)
    
    {
        _mapper = mapper;
        _passwordEncrypter = passwordEncrypter;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _tokenRepository = tokenRepository;
    }
    public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
    {
        await Validate(request);

        var user = _mapper.Map<Domain.Entities.User>(request);
        user.Password = _passwordEncrypter.Encrypt(request.Password);
        
        var refreshToken = await CreateAndSaveRefreshToken(user);

        await _userRepository.Add(user);

        await _unitOfWork.Commit();

        return new ResponseRegisteredUserJson
        {
            UserName = request.UserName,
            Tokens = new ResponseTokensJson
            {
                AccessToken = _accessTokenGenerator.Generate(user.Id, user.TokenVersion,user.Role),
                RefreshToken = refreshToken
            }
        };
    }
    
    private async Task<string> CreateAndSaveRefreshToken(Domain.Entities.User user)
    {
        var refreshToken = new Domain.Entities.RefreshToken
        {
            Token = _refreshTokenGenerator.Generate(),
            UserId = user.Id
        };

        await _tokenRepository.SaveNewRefreshToken(refreshToken);

        return refreshToken.Token;
    }

    private async Task Validate(RequestRegisterUserJson request)
    {
        var validator = new  RegisterUserValidator();
        
        var result =  await validator.ValidateAsync(request);
        
        if(request.Password != request.ConfirmPassword)
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.PASSWORDS_NOT_MATCH));
        
        var emailExist = await _userRepository.ExistsActiveUserWithEmail(request.Email);
        if(emailExist)
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.EMAIL_ALREADY_REGISTERED));
        
        var userNameExist = await _userRepository.ExistsActiveUserWithUserName(request.UserName);
        if(userNameExist)
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.USERNAME_ALREADY_REGISTERED));
            
        
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
        
    }
}