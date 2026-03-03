using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.Anime.Update;
using UserAnimeList.Domain.Extensions;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Anime.Update;

public class UpdateAnimeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var studio = StudioBuilder.Build();
        var genre = GenreBuilder.Build();
        var anime = AnimeBuilder.Build();
        var request = RequestAnimeJsonBuilder.Build();
        request.Genres.Clear();
        request.Genres.Add(genre.Id);
        request.Studios.Clear();
        request.Studios.Add(studio.Id);

        var useCase = CreateUseCase(studio, genre, anime);
        
        Func<Task> act = async () => await useCase.Execute(request, anime.Id.ToString());

        await act();

        Assert.Equal(request.Name, anime.Name);
        Assert.Equal(request.Synopsis, anime.Synopsis);
    }
    
    [Fact]
    public async Task Error_Name_Empty()
    {
        var studio = StudioBuilder.Build();
        var genre = GenreBuilder.Build();
        var anime = AnimeBuilder.Build();
        var request = RequestAnimeJsonBuilder.Build();
        request.Genres.Clear();
        request.Genres.Add(genre.Id);
        request.Studios.Clear();
        request.Studios.Add(studio.Id);
        request.Name = string.Empty;

        var useCase = CreateUseCase(studio, genre, anime);
        
        Func<Task> act = async () => await useCase.Execute(request,anime.Id.ToString());

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.NAME_EMPTY, exception.GetErrorMessages().FirstOrDefault());
        
        Assert.NotEqual(request.Name,anime.Name);
        Assert.NotEqual(request.Synopsis,anime.Synopsis);
    }
    
    [Fact]
    public async Task Error_Name_Already_Registered()
    {
        var studio = StudioBuilder.Build();
        var genre = GenreBuilder.Build();
        var anime = AnimeBuilder.Build();
        var request = RequestAnimeJsonBuilder.Build();
        request.Genres.Clear();
        request.Genres.Add(genre.Id);
        request.Studios.Clear();
        request.Studios.Add(studio.Id);

        var useCase = CreateUseCase(studio, genre, anime, request.Name);
        
        Func<Task> act = async () => await useCase.Execute(request,anime.Id.ToString());
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.ANIME_ALREADY_REGISTERED, exception.GetErrorMessages().FirstOrDefault());
        
        Assert.NotEqual(request.Name,anime.Name);
        Assert.NotEqual(request.Synopsis,anime.Synopsis);
    }
    
    [Fact]
    public async Task Error_Anime_Not_Found()
    {
        var studio = StudioBuilder.Build();
        var genre = GenreBuilder.Build();
        var anime = AnimeBuilder.Build();
        var request = RequestAnimeJsonBuilder.Build();

        var useCase = CreateUseCase(studio, genre, anime);
        
        Func<Task> act = async () => await useCase.Execute(request,Guid.NewGuid().ToString());

        var exception = await Assert.ThrowsAsync<NotFoundException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.ANIME_NOT_FOUND, exception.GetErrorMessages().FirstOrDefault());
        
        Assert.NotEqual(request.Name,anime.Name);
        Assert.NotEqual(request.Synopsis,anime.Synopsis);
    }
    
    [Fact]
    public async Task Error_Studio_Invalid()
    {
        var studio = StudioBuilder.Build();
        var genre = GenreBuilder.Build();
        var anime = AnimeBuilder.Build();
        var request = RequestAnimeJsonBuilder.Build();
        request.Genres.Clear();
        request.Genres.Add(genre.Id);

        var useCase = CreateUseCase(studio, genre, anime);
        
        Func<Task> act = async ()  => await useCase.Execute(request, anime.Id.ToString());
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();
        var invalidStudios = String.Join(", ", request.Studios);
        Assert.Single(errors);
        Assert.Contains($"{ResourceMessagesException.INVALID_STUDIOS_IN_REQUEST}{invalidStudios}", errors);
    }
    
    [Fact]
    public async Task Error_Genre_Invalid()
    {
        var studio = StudioBuilder.Build();
        var genre = GenreBuilder.Build();
        var anime = AnimeBuilder.Build();
        var request = RequestAnimeJsonBuilder.Build();
        request.Studios.Clear();
        request.Studios.Add(studio.Id);

        var useCase = CreateUseCase(studio, genre, anime);
        
        Func<Task> act = async ()  => await useCase.Execute(request, anime.Id.ToString());
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();
        var invalidGenres = String.Join(", ", request.Genres);
        Assert.Single(errors);
        Assert.Contains($"{ResourceMessagesException.INVALID_GENRES_IN_REQUEST}{invalidGenres}", errors);
    }
    
    private static UpdateAnimeUseCase CreateUseCase(UserAnimeList.Domain.Entities.Studio studio, 
        UserAnimeList.Domain.Entities.Genre genre, 
        UserAnimeList.Domain.Entities.Anime anime, 
        string? name = null)
    {
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var animeRepositoryBuilder = new AnimeRepositoryBuilder();
        var animeRepository = animeRepositoryBuilder.WithAnime(anime).GetById(anime).Build();
        var studioRepositoryBuilder = new StudioRepositoryBuilder().WithStudio(studio).GetStudiosInList().Build();
        var genreRepositoryBuilder = new GenreRepositoryBuilder().WithGenre(genre).GetGenresInList().Build();
        
        
        if (name.NotEmpty())
            animeRepositoryBuilder.ExistsActiveAnimeWithId(name);

        
        return new UpdateAnimeUseCase(mapper,animeRepository,genreRepositoryBuilder,studioRepositoryBuilder, unitOfWork);

    }
}