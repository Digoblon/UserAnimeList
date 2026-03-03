using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.Token.RefreshToken;

public interface IUseRefreshTokenUseCase
{
    Task<ResponseTokensJson> Execute(RequestNewTokenJson request);
}