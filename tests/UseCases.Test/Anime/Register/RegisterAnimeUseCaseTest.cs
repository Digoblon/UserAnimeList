using Bogus.DataSets;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.Anime.Register;
using UserAnimeList.Domain.Extensions;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Anime.Register;

public class RegisterAnimeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var studio = StudioBuilder.Build();
        var genre =  GenreBuilder.Build();
        var request = RequestAnimeJsonBuilder.Build();
        request.Genres.Clear();
        request.Genres.Add(genre.Id);
        request.Studios.Clear();
        request.Studios.Add(studio.Id);

        var useCase = CreateUseCase(studio, genre);
        
        var result = await useCase.Execute(request);

        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
    }
    
    [Fact]
    public async Task Error_Name_Already_Exists()
    {
        var studio = StudioBuilder.Build();
        var genre =  GenreBuilder.Build();
        var request = RequestAnimeJsonBuilder.Build();
        request.Genres.Clear();
        request.Genres.Add(genre.Id);
        request.Studios.Clear();
        request.Studios.Add(studio.Id);

        var useCase = CreateUseCase(studio, genre,request.Name);
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();

        Assert.Single(errors);
        Assert.Contains(ResourceMessagesException.ANIME_ALREADY_REGISTERED, errors);
    }
    
    [Fact]
    public async Task Error_Name_Empty()
    {
        var studio = StudioBuilder.Build();
        var genre =  GenreBuilder.Build();
        var request = RequestAnimeJsonBuilder.Build();
        request.Genres.Clear();
        request.Genres.Add(genre.Id);
        request.Studios.Clear();
        request.Studios.Add(studio.Id);
        request.Name = string.Empty;

        var useCase = CreateUseCase(studio, genre,request.Name);
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();

        Assert.Single(errors);
        Assert.Contains(ResourceMessagesException.NAME_EMPTY, errors);
    }
    
    [Fact]
    public async Task Error_Studio_Invalid()
    {
        var studio = StudioBuilder.Build();
        var genre =  GenreBuilder.Build();
        var request = RequestAnimeJsonBuilder.Build();
        request.Genres.Clear();
        request.Genres.Add(genre.Id);


        var useCase = CreateUseCase(studio, genre);
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
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
        var genre =  GenreBuilder.Build();
        var request = RequestAnimeJsonBuilder.Build();
        request.Studios.Clear();
        request.Studios.Add(studio.Id);

        var useCase = CreateUseCase(studio, genre);
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();
        var invalidGenres = String.Join(", ", request.Genres);
        Assert.Single(errors);
        Assert.Contains($"{ResourceMessagesException.INVALID_GENRES_IN_REQUEST}{invalidGenres}", errors);
    }
    
    private static RegisterAnimeUseCase CreateUseCase(UserAnimeList.Domain.Entities.Studio studio, UserAnimeList.Domain.Entities.Genre genre, string? name = null)
    {
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var animeRepositoryBuilder = new AnimeRepositoryBuilder();
        var animeRepository = animeRepositoryBuilder.Build();
        var studioRepositoryBuilder = new StudioRepositoryBuilder().WithStudio(studio).GetStudiosInList().Build();
        var genreRepositoryBuilder = new GenreRepositoryBuilder().WithGenre(genre).GetGenresInList().Build();
        
        
        if (name.NotEmpty())
            animeRepositoryBuilder.ExistsActiveAnimeWithId(name);

        
        return new RegisterAnimeUseCase(mapper,animeRepository,genreRepositoryBuilder,studioRepositoryBuilder, unitOfWork);

    }
}