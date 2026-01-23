using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.Genre.Update;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Genre.Update;

public class UpdateGenreUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var genre = GenreBuilder.Build();

        var request = RequestUpdateGenreJsonBuilder.Build();

        var useCase = CreateUseCase(genre);
        
        Func<Task> act = async () => await useCase.Execute(request,genre.Id.ToString());

        await act();
        
        Assert.Equal(request.Name, genre.Name);
        Assert.Equal(request.Description, genre.Description);
    }
    
    [Fact]
    public async Task Error_Name_Empty()
    {
        var genre = GenreBuilder.Build();

        var request = RequestUpdateGenreJsonBuilder.Build();
        request.Name = string.Empty;

        var useCase = CreateUseCase(genre);
        
        Func<Task> act = async () => await useCase.Execute(request,genre.Id.ToString());

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.NAME_EMPTY, exception.GetErrorMessages().FirstOrDefault());
        
        Assert.NotEqual(request.Name,genre.Name);
        Assert.NotEqual(request.Description,genre.Description);
    }
    
    [Fact]
    public async Task Error_Name_Already_Registered()
    {
        var genre = GenreBuilder.Build();

        var request = RequestUpdateGenreJsonBuilder.Build();

        var useCase = CreateUseCase(genre,request.Name);
        
        Func<Task> act = async () => await useCase.Execute(request,genre.Id.ToString());
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.STUDIO_ALREADY_REGISTERED, exception.GetErrorMessages().FirstOrDefault());
        
        Assert.NotEqual(request.Name,genre.Name);
        Assert.NotEqual(request.Description,genre.Description);
    }


    private static UpdateGenreUseCase CreateUseCase(UserAnimeList.Domain.Entities.Genre genre, string? name = null)
    {
        
        var genreRepositoryBuilder = new GenreRepositoryBuilder();
        var genreRepository = genreRepositoryBuilder.GetById(genre).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        
        if(!string.IsNullOrEmpty(name))
            genreRepositoryBuilder.ExistsActiveGenreWithName(name);

        return new UpdateGenreUseCase(genreRepository,unitOfWork);
    }
}