using UserAnimeList.Communication.Requests;
using UserAnimeList.Communication.Responses;

namespace UserAnimeList.Application.UseCases.AnimeList.AddEntry;

public interface IAddAnimeListEntryUseCase
{
    public Task<ResponseAnimeListEntryJson> Execute(RequestAnimeListEntryJson request);
}