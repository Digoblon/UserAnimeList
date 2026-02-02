using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using UserAnimeList.Application.UseCases.Anime.Search;
using UserAnimeList.Communication.Requests;

namespace UseCases.Test.Anime.Search;

public class SearchAnimeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var anime = AnimeBuilder.Build();
        var animeList = AnimeBuilder.Collection();

        var useCase = CreateUseCase(anime, animeList);

        var request = new RequestAnimeSearchJson
        {
            Query = anime.Name
        };
        var result = await useCase.Execute(request);
        
        Assert.NotEmpty(result.Animes);
        Assert.Contains(anime.Name, result.Animes.Select(a => a.Name));
    }
    [Fact]
    public async Task Success_Query_All()
    {
        var anime = AnimeBuilder.Build();
        var animeList = AnimeBuilder.Collection();
    
        var useCase = CreateUseCase(anime, animeList);
    
        var request = new RequestAnimeSearchJson
        {
            Query = "*"
        };
        var result = await useCase.Execute(request);
        
        
        Assert.NotEmpty(result.Animes);
        Assert.Equal(animeList.Count +  1, result.Animes.Count);
    }
        
    [Theory]
    [InlineData("")]
    [InlineData("aa")]
    public async Task Error_Query_Invalid(string query)
    {
        var anime = AnimeBuilder.Build();
        var animeList = AnimeBuilder.Collection();

        var useCase = CreateUseCase(anime, animeList);

        var request = new RequestAnimeSearchJson
        {
            Query = query
        };
        var result = await useCase.Execute(request);
        
        Assert.Empty(result.Animes);
    }
    
    
    
    private static SearchAnimeUseCase CreateUseCase(UserAnimeList.Domain.Entities.Anime anime, IList<UserAnimeList.Domain.Entities.Anime>? animeList = null)
    {
        var mapper = MapperBuilder.Build();
        var animeRepositoryBuilder = new AnimeRepositoryBuilder();
        var animeRepository = animeRepositoryBuilder.WithAnime(anime).GetById(anime).Search().Build();
        
        if(animeList is not null)
            animeRepositoryBuilder.AddList(animeList);
        
        return new SearchAnimeUseCase(animeRepository,mapper);
    }
}