using FluentValidation.Results;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Security.Cryptography;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Domain.Services.LoggedUser;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.User.ChangePassword;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
        
    public ChangePasswordUseCase(ILoggedUser loggedUser,
        IUserRepository repository, 
        IUnitOfWork unitOfWork,
        IPasswordEncrypter passwordEncrypter,  
        IAccessTokenGenerator accessTokenGenerator)
    {
        _loggedUser = loggedUser;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _passwordEncrypter = passwordEncrypter;
        _accessTokenGenerator = accessTokenGenerator;
    }

    public async Task<ResponseChangePasswordJson> Execute(RequestChangePasswordJson request)
    {
        var user = await _loggedUser.User();
        
        Validate(request, user);
        
        user.Password = _passwordEncrypter.Encrypt(request.NewPassword);
        
        _repository.Update(user);
        
        await _unitOfWork.Commit();

        return new ResponseChangePasswordJson
        {
            Tokens = new ResponseTokensJson 
            {
                AccessToken = _accessTokenGenerator.Generate(user.Id)
            }
        };
    }

    private void Validate(RequestChangePasswordJson request, Domain.Entities.User loggedUser)
    {
        var result = new ChangePasswordValidator().Validate(request);
        
        
        if(request.NewPassword != request.ConfirmNewPassword)
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.PASSWORDS_NOT_MATCH));
        
        if(!_passwordEncrypter.IsValid(request.Password,loggedUser.Password))
            result.Errors.Add(new ValidationFailure(string.Empty,ResourceMessagesException.PASSWORD_DIFFERENT_CURRENT_PASSWORD));

        if (!result.IsValid)
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }
}