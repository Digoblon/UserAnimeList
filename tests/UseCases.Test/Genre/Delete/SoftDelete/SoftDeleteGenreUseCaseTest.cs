using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using UserAnimeList.Application.UseCases.Genre.Delete.SoftDelete;

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
    public async Task Error_User_Not_Active()
    {
        var genre = GenreBuilder.Build();
        genre.IsActive = false;

        var useCase = CreateUseCase(genre);
        
        Func<Task> act = async () => await useCase.Execute(genre.Id.ToString());

        await act();
    }
    
    
    private static SoftDeleteGenreUseCase CreateUseCase(UserAnimeList.Domain.Entities.Genre genre)
    {
        var genreRepositoryBuilder = new GenreRepositoryBuilder();
        var genreRepository = genreRepositoryBuilder.GetById(genre).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new SoftDeleteGenreUseCase(genreRepository, unitOfWork);
    }
}