using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.Anime.Image.Update;

public interface IUpdateAnimeImageUseCase
{
    Task<ResponseUpdateImageJson> Execute (RequestUpdateImageFormData request, string id);
}