using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.User.Profile;

public interface IGetUserProfileUseCase
{
    public Task<ResponseUserProfileJson> Execute();
}