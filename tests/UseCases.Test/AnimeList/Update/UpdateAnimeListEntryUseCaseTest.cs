using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.AnimeList.Update;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.AnimeList.Update;

public class UpdateAnimeListEntryUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var anime = AnimeBuilder.Build();
        var (user, _) =  UserBuilder.Build();
        var animeList = AnimeListBuilder.Build(anime, user);
        var request = RequestUpdateAnimeListEntryJsonBuilder.Build();

        var useCase = CreateUseCase(user, animeList);
        
        Func<Task> act = async ()  => await useCase.Execute(request, animeList.Id.ToString());

        await act();
    }
    
    [Fact]
    public async Task Error_Progress_Invalid()
    {
        var anime = AnimeBuilder.Build();
        var (user, _) =  UserBuilder.Build();
        var animeList = AnimeListBuilder.Build(anime, user);
        var request = RequestUpdateAnimeListEntryJsonBuilder.Build();
        anime.Episodes = 10;
        request.Progress = 100;
        
        var useCase = CreateUseCase(user, animeList);
        
        Func<Task> act = async ()  => await useCase.Execute(request,  animeList.Id.ToString());
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.EPISODE_COUNT_INVALID, errors.First());
    }
    
    [Fact]
    public async Task Error_AnimeList_Not_Found()
    {
        var anime = AnimeBuilder.Build();
        var (user, _) =  UserBuilder.Build();
        var animeList = AnimeListBuilder.Build(anime, user);
        var request = RequestUpdateAnimeListEntryJsonBuilder.Build();
        
        var useCase = CreateUseCase(user, animeList);
        
        Func<Task> act = async ()  => await useCase.Execute(request,  Guid.NewGuid().ToString());
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(act);

        var errors = exception.GetErrorMessages();
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_LIST_INVALID, errors.First());
    }
    
    [Fact]
    public async Task Error_Status_Invalid()
    {
        var anime = AnimeBuilder.Build();
        var (user, _) =  UserBuilder.Build();
        var animeList = AnimeListBuilder.Build(anime, user);
        var request = RequestUpdateAnimeListEntryJsonBuilder.Build();
        request.Status = (AnimeEntryStatus)100;
        
        var useCase = CreateUseCase(user, animeList);
        
        Func<Task> act = async ()  => await useCase.Execute(request, animeList.Id.ToString());
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_LIST_INVALID_STATUS, errors.First());
    }
    
    
    
    private static UpdateAnimeListEntryUseCase CreateUseCase(UserAnimeList.Domain.Entities.User user,UserAnimeList.Domain.Entities.AnimeList animeList)
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var animeListRepositoryBuilder = new AnimeListRepositoryBuilder();
        var animeListRepository = animeListRepositoryBuilder.GetById(animeList.Id.ToString(), user, animeList).Build();

        
        return new UpdateAnimeListEntryUseCase(mapper,animeListRepository,loggedUser, unitOfWork);

    }
}