using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.Anime.Filter;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Anime.Filter;

public class AnimeFilterUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var anime = AnimeBuilder.Build();
        var animeList = AnimeBuilder.Collection();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.Query = anime.Name;

        var useCase = CreateUseCase(anime, animeList);

        var response = await useCase.Execute(request);

        Assert.NotNull(response);
        Assert.NotNull(response.Animes);
        Assert.NotEmpty(response.Animes);
        Assert.Contains(response.Animes, a => a.Name == anime.Name);
    }

    [Fact]
    public async Task Success_Filter_Returns_Empty()
    {
        var anime = AnimeBuilder.Build();
        var animeList = AnimeBuilder.Collection();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.Query = "NoMatchAnimeName";

        var useCase = CreateUseCase(anime, animeList);

        var response = await useCase.Execute(request);

        Assert.NotNull(response);
        Assert.NotNull(response.Animes);
        Assert.Empty(response.Animes);
    }

    [Fact]
    public async Task Error_Status_Invalid()
    {
        var anime = AnimeBuilder.Build();
        var animeList = AnimeBuilder.Collection();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.Status = (AnimeStatus)100;

        var useCase = CreateUseCase(anime, animeList);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        var errors = exception.GetErrorMessages();

        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.STATUS_NOT_SUPPORTED, errors.First());
    }

    [Fact]
    public async Task Error_Query_Exceeded()
    {
        var anime = AnimeBuilder.Build();
        var animeList = AnimeBuilder.Collection();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.Query = RequestStringGenerator.Paragraphs(minCharacters: 257);

        var useCase = CreateUseCase(anime, animeList);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        var errors = exception.GetErrorMessages();

        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_QUERY_EXCEEDED, errors.First());
    }

    [Fact]
    public async Task Error_Duplicated_Genres()
    {
        var anime = AnimeBuilder.Build();
        var animeList = AnimeBuilder.Collection();
        var repeatedGenre = Guid.NewGuid();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.Genres = [repeatedGenre, repeatedGenre];

        var useCase = CreateUseCase(anime, animeList);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        var errors = exception.GetErrorMessages();

        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.DUPLICATED_GENRES, errors.First());
    }

    [Fact]
    public async Task Error_Premiered_Season_Without_Year()
    {
        var anime = AnimeBuilder.Build();
        var animeList = AnimeBuilder.Collection();
        var request = RequestAnimeFilterJsonBuilder.Build();
        request.PremieredSeason = Season.Winter;
        request.PremieredYear = null;

        var useCase = CreateUseCase(anime, animeList);

        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        var errors = exception.GetErrorMessages();

        Assert.Single(errors);
        Assert.Equal(ResourceMessagesException.ANIME_PREMIERED_YEAR_REQUIRED, errors.First());
    }

    private static FilterAnimeUseCase CreateUseCase(UserAnimeList.Domain.Entities.Anime anime, IList<UserAnimeList.Domain.Entities.Anime>? animeList = null)
    {
        var mapper = MapperBuilder.Build();
        var animeRepositoryBuilder = new AnimeRepositoryBuilder();
        var animeRepository = animeRepositoryBuilder.WithAnime(anime).Filter().Build();

        if (animeList is not null)
            animeRepositoryBuilder.AddList(animeList);

        return new FilterAnimeUseCase(animeRepository, mapper);
    }
}
