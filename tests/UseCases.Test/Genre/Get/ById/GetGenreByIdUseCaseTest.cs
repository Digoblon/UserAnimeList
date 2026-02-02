using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using UserAnimeList.Application.UseCases.Genre.Get.ById;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Genre.Get.ById;

public class GetGenreByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var genre = GenreBuilder.Build();

        var useCase = CreateUseCase(genre);
        
        var result = await useCase.Execute(genre.Id.ToString());

        Assert.NotNull(result);
        Assert.Equal(genre.Name, result.Name);
        Assert.Equal(genre.Description, result.Description);
    }
    
    [Fact]
    public async Task Error_Genre_Not_Found()
    {
        var genre = GenreBuilder.Build();

        var useCase = CreateUseCase(genre);

        var genreId = Guid.NewGuid();
        
        Func<Task> act = async () => await useCase.Execute(genreId.ToString());
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(act);

        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.GENRE_NOT_FOUND, exception.GetErrorMessages().FirstOrDefault());
    }

    private static GetGenreByIdUseCase CreateUseCase(UserAnimeList.Domain.Entities.Genre genre)
    {
        var mapper = MapperBuilder.Build();
        var genreRepositoryBuilder = new GenreRepositoryBuilder();
        var genreRepository = genreRepositoryBuilder.GetById(genre).Build();
        
        return new GetGenreByIdUseCase(genreRepository, mapper);
    }
}