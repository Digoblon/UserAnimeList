using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.User.Register;

public interface IRegisterUserUseCase
{
    public Task<ResponseRegisterUser> Execute(RequestRegisterUserJson request);
}