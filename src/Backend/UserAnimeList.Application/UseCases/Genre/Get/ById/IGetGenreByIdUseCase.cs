using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.Genre.Get.ById;

public interface IGetGenreByIdUseCase
{
    public Task<ResponseGenreJson>  Execute(string id);
}