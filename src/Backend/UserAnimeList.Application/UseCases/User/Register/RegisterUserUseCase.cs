using FluentValidation.Results;
using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Security.Cryptography;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.User.Register;

public class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IAppMapper _mapper;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserUseCase(IAppMapper mapper, 
        IPasswordEncrypter passwordEncrypter,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    
    {
        _mapper = mapper;
        _passwordEncrypter = passwordEncrypter;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }
    public async Task<ResponseRegisterUser> Execute(RequestRegisterUserJson request)
    {
        await Validate(request);

        var user = _mapper.Map<Domain.Entities.User>(request);
        user.Password = _passwordEncrypter.Encrypt(request.Password);

        await _userRepository.AddAsync(user);

        await _unitOfWork.Commit();

        return new ResponseRegisterUser
        {
            UserName = request.UserName
        };
    }

    private async Task Validate(RequestRegisterUserJson request)
    {
        var validator = new  RegisterUserValidator();
        
        var result =  await validator.ValidateAsync(request);
        
        var emailExist = await _userRepository.ExistsActiveUserWithEmailAsync(request.Email);
        if(emailExist)
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.EMAIL_ALREADY_REGISTERED));
        
        var userNameExist = await _userRepository.ExistsActiveUserWithUserNameAsync(request.UserName);
        if(userNameExist)
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.USERNAME_ALREADY_REGISTERED));
            
        
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).Distinct().ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
        
    }
}