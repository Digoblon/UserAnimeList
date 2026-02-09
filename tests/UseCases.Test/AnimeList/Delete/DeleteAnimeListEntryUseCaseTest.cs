using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using UserAnimeList.Application.UseCases.AnimeList.Delete;
using UserAnimeList.Application.UseCases.AnimeList.Get.ById;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.AnimeList.Delete;

public class DeleteAnimeListEntryUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var anime = AnimeBuilder.Build();
        var (user, _) =  UserBuilder.Build();
        var animeList = AnimeListBuilder.Build(anime, user);

        var useCase = CreateUseCase(user, animeList);
        
        Func<Task> act = async ()  => await useCase.Execute(animeList.Id.ToString());

        await act();
    }
    
    [Fact]
    public async Task Error_AnimeList_Not_Found()
    {
        var anime = AnimeBuilder.Build();
        var (user, _) =  UserBuilder.Build();
        var animeList = AnimeListBuilder.Build(anime, user);
        
        var useCase = CreateUseCase(user, animeList);
        
        Func<Task> act = async ()  => await useCase.Execute(Guid.NewGuid().ToString());
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(act);

        var errors = exception.GetErrorMessages();
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_LIST_INVALID, errors.First());
    }
    
    private static DeleteAnimeListEntryUseCase CreateUseCase(UserAnimeList.Domain.Entities.User user,UserAnimeList.Domain.Entities.AnimeList animeList)
    {
        
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var animeListRepositoryBuilder = new AnimeListRepositoryBuilder();
        var animeListRepository = animeListRepositoryBuilder.GetById(animeList.Id.ToString(), user, animeList).Build();

        
        return new DeleteAnimeListEntryUseCase(loggedUser,animeListRepository, unitOfWork);

    }
}