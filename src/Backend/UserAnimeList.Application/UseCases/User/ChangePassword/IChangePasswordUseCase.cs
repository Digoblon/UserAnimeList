using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.User.ChangePassword;

public interface IChangePasswordUseCase
{
    public Task<ResponseTokensJson> Execute(RequestChangePasswordJson request);
}