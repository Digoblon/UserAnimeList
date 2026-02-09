using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.AnimeList.List.ByUserId;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.AnimeList.List.ById;

public class ListAnimeByUserIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) =  UserBuilder.Build();
        var animeLists = AnimeListBuilder.Collection(user);
        var request = RequestAnimeListEntryFilterJsonBuilder.Build();

        var useCase = CreateUseCase(user, animeLists);
        
        var response =  await useCase.Execute(user.Id.ToString(),request);
        
        Assert.NotNull(response);
        Assert.NotNull(response.Lists);
        Assert.NotEmpty(response.Lists);
        Assert.Equal(response.Lists.Count, animeLists.Count);
    }
    
    [Fact]
    public async Task Error_Status_Invalid()
    {
        var (user, _) =  UserBuilder.Build();
        var animeLists = AnimeListBuilder.Collection(user);
        var request = RequestAnimeListEntryFilterJsonBuilder.Build();
        request.Status = (AnimeEntryStatus)100;
        var useCase = CreateUseCase(user, animeLists);
        
        Func<Task> act = async ()  => await useCase.Execute(user.Id.ToString(),request);
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();
        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_LIST_INVALID_STATUS, errors.First());
    }
    
    private static ListAnimeByUserIdUseCase CreateUseCase(UserAnimeList.Domain.Entities.User user,IList<UserAnimeList.Domain.Entities.AnimeList> animeLists)
    {
        var mapper = MapperBuilder.Build();
        var userRepositoryBuilder = new UserRepositoryBuilder();
        var userRepository = userRepositoryBuilder.GetById(user).Build();
        var animeListRepositoryBuilder = new AnimeListRepositoryBuilder();
        var animeListRepository = animeListRepositoryBuilder.WithLists(animeLists).List(user).Build();

        
        return new ListAnimeByUserIdUseCase(mapper,userRepository,animeListRepository);

    }
}