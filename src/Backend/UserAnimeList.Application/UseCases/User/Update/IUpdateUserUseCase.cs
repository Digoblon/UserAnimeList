using UserAnimeList.Communication.Requests;

namespace UserAnimeList.Application.UseCases.User.Update;

public interface IUpdateUserUseCase
{
    public Task Execute(RequestUpdateUserJson request);
}