using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using UserAnimeList.Application.UseCases.Studio.Delete.SoftDelete;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Studio.Delete.SoftDelete;

public class SoftDeleteStudioUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var studio = StudioBuilder.Build();

        var useCase = CreateUseCase(studio);
        
        Func<Task> act = async () => await useCase.Execute(studio.Id.ToString());

        await act();
    }
    
    [Fact]
    public async Task Error_Genre_Not_Active()
    {
        var studio = StudioBuilder.Build();
        studio.IsActive = false;

        var useCase = CreateUseCase(studio);
        
        Func<Task> act = async () => await useCase.Execute(studio.Id.ToString());

        await act();
    }
    
    [Fact]
    public async Task Error_Studio_Not_Found()
    {
        var studio = StudioBuilder.Build();

        var useCase = CreateUseCase(studio);
        
        Func<Task> act = async () => await useCase.Execute(Guid.NewGuid().ToString());


        var exception = await Assert.ThrowsAsync<NotFoundException>(act);
        
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.STUDIO_NOT_FOUND, exception.GetErrorMessages().FirstOrDefault());
    }
    
    
    private static SoftDeleteStudioUseCase CreateUseCase(UserAnimeList.Domain.Entities.Studio studio)
    {
        var studioRepositoryBuilder = new StudioRepositoryBuilder();
        var studioRepository = studioRepositoryBuilder.GetById(studio).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        return new SoftDeleteStudioUseCase(studioRepository, unitOfWork);
    }
}