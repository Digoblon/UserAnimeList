using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using UserAnimeList.Application.UseCases.Anime.Get.ById;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Anime.Get.ById;

public class GetAnimeByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var anime = AnimeBuilder.Build();

        var useCase = CreateUseCase(anime);
        
        var result = await useCase.Execute(anime.Id.ToString());

        Assert.NotNull(result);
        Assert.Equal(anime.Name, result.Name);
        Assert.Equal(anime.Synopsis, result.Synopsis);
    }
    
    [Fact]
    public async Task Error_Anime_Not_Found()
    {
        var anime = AnimeBuilder.Build();

        var useCase = CreateUseCase(anime);

        var animeId = Guid.NewGuid();
        
        Func<Task> act = async () => await useCase.Execute(animeId.ToString());
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(act);

        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.ANIME_NOT_FOUND, exception.GetErrorMessages().FirstOrDefault());
    }

    private static GetAnimeByIdUseCase CreateUseCase(UserAnimeList.Domain.Entities.Anime anime)
    {
        var mapper = MapperBuilder.Build();
        var animeRepositoryBuilder = new AnimeRepositoryBuilder();
        var animeRepository = animeRepositoryBuilder.WithAnime(anime).GetById(anime).Build();
        
        return new GetAnimeByIdUseCase(mapper,animeRepository);
    }
}