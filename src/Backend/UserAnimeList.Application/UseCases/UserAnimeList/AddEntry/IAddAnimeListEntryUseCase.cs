using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.UserAnimeList.AddEntry;

public interface IAddAnimeListEntryUseCase
{
    public Task<ResponseAnimeListEntryJson> Execute(RequestAnimeListEntryJson request);
}