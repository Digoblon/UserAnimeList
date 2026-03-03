using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.User.Image.Update;

public interface IUpdateUserImageUseCase
{
    Task<ResponseUpdateImageJson> Execute(RequestUpdateImageFormData  request);
}