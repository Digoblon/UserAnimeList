using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using UserAnimeList.Application.UseCases.Studio.Get.ById;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Studio.Get.ById;

public class GetStudioByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var studio = StudioBuilder.Build();

        var useCase = CreateUseCase(studio);
        
        var result = await useCase.Execute(studio.Id.ToString());

        Assert.NotNull(result);
        Assert.Equal(studio.Name, result.Name);
        Assert.Equal(studio.Description, result.Description);
    }
    
    [Fact]
    public async Task Error_Studio_Not_Found()
    {
        var studio = StudioBuilder.Build();

        var useCase = CreateUseCase(studio);

        var studioId = Guid.NewGuid();
        
        Func<Task> act = async () => await useCase.Execute(studioId.ToString());
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(act);

        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.STUDIO_NOT_FOUND, exception.GetErrorMessages().FirstOrDefault());
    }

    private static GetStudioByIdUseCase CreateUseCase(UserAnimeList.Domain.Entities.Studio studio)
    {
        var mapper = MapperBuilder.Build();
        var studioRepositoryBuilder = new StudioRepositoryBuilder();
        var studioRepository = studioRepositoryBuilder.GetById(studio).Build();
        
        return new GetStudioByIdUseCase(studioRepository, mapper);
    }
}