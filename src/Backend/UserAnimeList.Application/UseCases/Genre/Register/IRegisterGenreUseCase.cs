using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.Genre.Register;

public interface IRegisterGenreUseCase
{
    public Task<ResponseRegisteredGenreJson> Execute(RequestRegisterGenreJson request);
}