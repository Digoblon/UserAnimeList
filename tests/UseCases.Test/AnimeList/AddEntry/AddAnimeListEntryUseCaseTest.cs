using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.AnimeList.AddEntry;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.AnimeList.AddEntry;

public class AddAnimeListEntryUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var anime = AnimeBuilder.Build();
        var (user, _) =  UserBuilder.Build();
        var request = RequestAnimeListEntryJsonBuilder.Build();
        request.AnimeId = anime.Id;

        var useCase = CreateUseCase(user, anime);
        
        var result = await useCase.Execute(request);

        Assert.NotNull(result);
        Assert.Equal(request.AnimeId, result.AnimeId);
    }
    
    [Fact]
    public async Task Error_Entry_Exists()
    {
        var anime = AnimeBuilder.Build();
        var (user, _) =  UserBuilder.Build();
        var request = RequestAnimeListEntryJsonBuilder.Build();
        request.AnimeId = anime.Id;

        var useCase = CreateUseCase(user, anime, true);
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_LIST_ENTRY_ALREADY_EXISTS, errors.First());
    }
    
    [Fact]
    public async Task Error_Progress_Invalid()
    {
        var anime = AnimeBuilder.Build();
        var (user, _) =  UserBuilder.Build();
        var request = RequestAnimeListEntryJsonBuilder.Build();
        request.AnimeId = anime.Id;
        anime.Episodes = 10;
        request.Progress = 100;

        var useCase = CreateUseCase(user, anime);
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.EPISODE_COUNT_INVALID, errors.First());
    }
    
    [Fact]
    public async Task Error_Anime_Not_Found()
    {
        var anime = AnimeBuilder.Build();
        var (user, _) =  UserBuilder.Build();
        var request = RequestAnimeListEntryJsonBuilder.Build();

        var useCase = CreateUseCase(user, anime);
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(act);

        var errors = exception.GetErrorMessages();
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_NOT_FOUND, errors.First());
    }
    
    [Fact]
    public async Task Error_Status_Invalid()
    {
        var anime = AnimeBuilder.Build();
        var (user, _) =  UserBuilder.Build();
        var request = RequestAnimeListEntryJsonBuilder.Build();
        request.AnimeId = anime.Id;
        request.Status = (AnimeEntryStatus)100;


        var useCase = CreateUseCase(user, anime);
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_LIST_INVALID_STATUS, errors.First());
    }
    
    
    
    private static AddAnimeListEntryUseCase CreateUseCase(UserAnimeList.Domain.Entities.User user,UserAnimeList.Domain.Entities.Anime anime, bool listExist = false)
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var animeRepositoryBuilder = new AnimeRepositoryBuilder();
        var animeRepository = animeRepositoryBuilder.GetById(anime).Build();
        var animeListRepositoryBuilder = new AnimeListRepositoryBuilder();
        var animeListRepository = animeListRepositoryBuilder.Build();
        
        if (listExist)
            animeListRepositoryBuilder.ExistsEntry(anime, user);

        
        return new AddAnimeListEntryUseCase(mapper,loggedUser,animeRepository,animeListRepository, unitOfWork);

    }
}