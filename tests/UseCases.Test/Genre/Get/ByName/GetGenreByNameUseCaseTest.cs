using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using UserAnimeList.Application.UseCases.Genre.Get.ByName;
using UserAnimeList.Communication.Requests;

namespace UseCases.Test.Genre.Get.ByName;

public class GetGenreByNameUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var genre = GenreBuilder.Build();

        var useCase = CreateUseCase(genre);

        var request = new RequestGenreGetByNameJson
        {
            Name = genre.Name
        };
        var result = await useCase.Execute(request);
        
        Assert.NotEmpty(result.Genres);
        Assert.Single(result.Genres);
        Assert.Equal(genre.Name, result.Genres.First().Name);
        Assert.Equal(genre.Description, result.Genres.First().Description);
    }
    
    [Fact]
    public async Task Genres_Empty()
    {
        var genre = GenreBuilder.Build();

        var useCase = CreateUseCase(genre);

        var request = new RequestGenreGetByNameJson
        {
            Name = "aaaaaaaaaa"
        };
        
        var result = await useCase.Execute(request);
        
        Assert.Empty(result.Genres);
    }

    private static GetGenreByNameUseCase CreateUseCase(UserAnimeList.Domain.Entities.Genre genre)
    {
        var mapper = MapperBuilder.Build();

        var genreRepository = new GenreRepositoryBuilder().SearchByName(genre).Build();

        return new GetGenreByNameUseCase(genreRepository, mapper);
    }
}