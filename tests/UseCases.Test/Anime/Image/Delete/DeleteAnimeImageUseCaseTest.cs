using CommonTestUtilities.Entities;
using CommonTestUtilities.FileStorage;
using CommonTestUtilities.Repositories;
using UserAnimeList.Application.UseCases.Anime.Image.Delete;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Anime.Image.Delete;

public class DeleteAnimeImageUseCaseTest
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
    public async Task Error_Anime_Not_Found()
    {
        var anime = AnimeBuilder.Build();

        var useCase = CreateUseCase(anime);
        
        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid().ToString());

        var exception = await Assert.ThrowsAsync<NotFoundException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.ANIME_NOT_FOUND, exception.GetErrorMessages().FirstOrDefault());
    }
    
    
    
    private static DeleteAnimeImageUseCase CreateUseCase(UserAnimeList.Domain.Entities.Anime anime)
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var animeRepositoryBuilder = new AnimeRepositoryBuilder();
        var animeRepository = animeRepositoryBuilder.GetById(anime).Build();
        var fileStorageBuilder = new FileStorageBuilder();
        var fileStorage = fileStorageBuilder.Save().Build();
        
        return new DeleteAnimeImageUseCase(animeRepository,fileStorage,unitOfWork);

    }
}