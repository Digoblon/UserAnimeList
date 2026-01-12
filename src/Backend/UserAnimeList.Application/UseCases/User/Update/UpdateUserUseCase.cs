using FluentValidation.Results;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Services.LoggedUser;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UserAnimeList.Application.UseCases.User.Update;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserUseCase(
        ILoggedUser loggedUser,
        IUserRepository  repository,
        IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute(RequestUpdateUserJson request)
    {
        var user = await _loggedUser.User();
        
        await Validate(request,user.Email);
        
        user.Email = request.Email;
        user.UserName = request.UserName;
        
        _repository.Update(user);
        
        await _unitOfWork.Commit();

    }

    private async Task Validate(RequestUpdateUserJson request, string currentEmail)
    {
        var validator = new UpdateUserValidator();
        
        var result = await validator.ValidateAsync(request);
        

        if (!currentEmail.Equals(request.Email))
        {
            var userExist = await _repository.ExistsActiveUserWithEmail(request.Email);
            if (userExist)
                result.Errors.Add(new ValidationFailure("email",ResourceMessagesException.EMAIL_ALREADY_REGISTERED));
        }

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}