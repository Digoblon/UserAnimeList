using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using UserAnimeList.Application.UseCases.Studio.Get.ByName;
using UserAnimeList.Communication.Requests;

namespace UseCases.Test.Studio.Get.ByName;

public class GetStudioByNameUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var studio = StudioBuilder.Build();

        var useCase = CreateUseCase(studio);

        var request = new RequestStudioGetByName
        {
            Name = studio.Name
        };
        var result = await useCase.Execute(request);
        
        Assert.NotNull(result.Studios);
        Assert.Single(result.Studios);
        Assert.Equal(studio.Name, result.Studios.First().Name);
        Assert.Equal(studio.Description, result.Studios.First().Description);
    }
    
    [Fact]
    public async Task Studios_Empty()
    {
        var studio = StudioBuilder.Build();

        var useCase = CreateUseCase(studio);

        var request = new RequestStudioGetByName
        {
            Name = "aaaaaaaaaa"
        };
        
        var result = await useCase.Execute(request);
        
        Assert.Empty(result.Studios);
    }

    private static GetStudioByNameUseCase CreateUseCase(UserAnimeList.Domain.Entities.Studio studio)
    {
        var mapper = MapperBuilder.Build();

        var studioRepository = new StudioRepositoryBuilder().SearchByName(studio).Build();

        return new GetStudioByNameUseCase(studioRepository, mapper);
    }
}