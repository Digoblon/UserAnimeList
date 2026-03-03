using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using UserAnimeList.Application.UseCases.Genre.Delete.SoftDelete;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Genre.Delete.SoftDelete;

public class SoftDeleteGenreUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var genre = GenreBuilder.Build();

        var useCase = CreateUseCase(genre);
        
        Func<Task> act = async () => await useCase.Execute(genre.Id.ToString());

        await act();
    }
    
    [Fact]
    public async Task Error_Genre_Not_Active()
    {
        var genre = GenreBuilder.Build();
        genre.IsActive = false;

        var useCase = CreateUseCase(genre);
        
        Func<Task> act = async () => await useCase.Execute(genre.Id.ToString());

        await act();
    }
    
    [Fact]
    public async Task Error_Genre_Not_Found()
    {
        var genre = GenreBuilder.Build();

        var useCase = CreateUseCase(genre);
        
        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid().ToString());

        var exception = await Assert.ThrowsAsync<NotFoundException>(act);
        
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.GENRE_NOT_FOUND, exception.GetErrorMessages().FirstOrDefault());
    }
    
    
    private static SoftDeleteGenreUseCase CreateUseCase(UserAnimeList.Domain.Entities.Genre genre)
    {
        var genreRepositoryBuilder = new GenreRepositoryBuilder();
        var genreRepository = genreRepositoryBuilder.GetById(genre).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new SoftDeleteGenreUseCase(genreRepository, unitOfWork);
    }
}