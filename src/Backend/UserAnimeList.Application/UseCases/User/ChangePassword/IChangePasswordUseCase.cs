using UserAnimeList.Communication.Requests;

namespace UserAnimeList.Application.UseCases.User.ChangePassword;

public interface IChangePasswordUseCase
{
    public Task Execute(RequestChangePasswordJson request);
}