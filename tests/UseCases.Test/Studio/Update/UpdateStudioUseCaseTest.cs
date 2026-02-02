using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.Studio.Update;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Studio.Update;

public class UpdateStudioUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var studio = StudioBuilder.Build();

        var request = RequestUpdateStudioJsonBuilder.Build();

        var useCase = CreateUseCase(studio);
        
        Func<Task> act = async () => await useCase.Execute(request,studio.Id.ToString());

        await act();
        
        Assert.Equal(request.Name, studio.Name);
        Assert.Equal(request.Description, studio.Description);
    }
    
    [Fact]
    public async Task Error_Name_Empty()
    {
        var studio = StudioBuilder.Build();

        var request = RequestUpdateStudioJsonBuilder.Build();
        request.Name = string.Empty;

        var useCase = CreateUseCase(studio);
        
        Func<Task> act = async () => await useCase.Execute(request,studio.Id.ToString());

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.NAME_EMPTY, exception.GetErrorMessages().FirstOrDefault());
        
        Assert.NotEqual(request.Name,studio.Name);
        Assert.NotEqual(request.Description,studio.Description);
    }
    
    [Fact]
    public async Task Error_Name_Already_Registered()
    {
        var studio = StudioBuilder.Build();

        var request = RequestUpdateStudioJsonBuilder.Build();

        var useCase = CreateUseCase(studio,request.Name);
        
        Func<Task> act = async () => await useCase.Execute(request,studio.Id.ToString());
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.STUDIO_ALREADY_REGISTERED, exception.GetErrorMessages().FirstOrDefault());
        
        Assert.NotEqual(request.Name,studio.Name);
        Assert.NotEqual(request.Description,studio.Description);
    }
    
    [Fact]
    public async Task Error_Studio_Not_Found()
    {
        var studio = StudioBuilder.Build();

        var request = RequestUpdateStudioJsonBuilder.Build();

        var useCase = CreateUseCase(studio,request.Name);
        
        Func<Task> act = async () => await useCase.Execute(request,Guid.NewGuid().ToString());
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.STUDIO_NOT_FOUND, exception.GetErrorMessages().FirstOrDefault());
        
        Assert.NotEqual(request.Name,studio.Name);
        Assert.NotEqual(request.Description,studio.Description);
    }


    private static UpdateStudioUseCase CreateUseCase(UserAnimeList.Domain.Entities.Studio studio, string? name = null)
    {
        
        var studioRepositoryBuilder = new StudioRepositoryBuilder();
        var studioRepository = studioRepositoryBuilder.GetById(studio).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        
        if(!string.IsNullOrEmpty(name))
            studioRepositoryBuilder.ExistsActiveStudioWithName(name);

        return new UpdateStudioUseCase(studioRepository,unitOfWork);
    }
}