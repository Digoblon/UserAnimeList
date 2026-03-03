using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using UserAnimeList.Application.UseCases.Anime.Delete.SoftDelete;
using UserAnimeList.Domain.Extensions;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Anime.Delete.SoftDelete;

public class SoftDeleteAnimeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var anime = AnimeBuilder.Build();

        var useCase = CreateUseCase(anime);
        
        Func<Task> act = async () => await useCase.Execute(anime.Id.ToString());

        await act();
    }
    
    [Fact]
    public async Task Error_Anime_Not_Active()
    {
        var anime = AnimeBuilder.Build();
        anime.IsActive = false;

        var useCase = CreateUseCase(anime);
        
        Func<Task> act = async () => await useCase.Execute(anime.Id.ToString());

        await act();
    }
    
    [Fact]
    public async Task Error_Anime_Not_Found()
    {
        var anime = AnimeBuilder.Build();

        var useCase = CreateUseCase(anime);
        
        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid().ToString());

        
        var exception = await Assert.ThrowsAsync<NotFoundException>(act);
        
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.ANIME_NOT_FOUND, exception.GetErrorMessages().FirstOrDefault());
    }
    
    
    private static SoftDeleteAnimeUseCase CreateUseCase(UserAnimeList.Domain.Entities.Anime anime)
    {
        var animeRepositoryBuilder = new AnimeRepositoryBuilder();
        var animeRepository = animeRepositoryBuilder.WithAnime(anime).GetById(anime).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        
        return new SoftDeleteAnimeUseCase(animeRepository, unitOfWork);
    }
}