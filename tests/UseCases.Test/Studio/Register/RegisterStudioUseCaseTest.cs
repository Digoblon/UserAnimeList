using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.Studio.Register;
using UserAnimeList.Domain.Extensions;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Studio.Register;

public class RegisterStudioUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterStudioJsonBuilder.Build();

        var useCase = CreateUseCase();
        
        var result = await useCase.Execute(request);

        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
    }
    
    [Fact]
    public async Task Error_Name_Already_Registered()
    {
        var request = RequestRegisterStudioJsonBuilder.Build();
        
        var useCase = CreateUseCase(request.Name);
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();

        Assert.Single(errors);
        Assert.Contains(ResourceMessagesException.STUDIO_ALREADY_REGISTERED, errors);
    }
    
    [Fact]
    public async Task Error_Name_Empty()
    {
        var request = RequestRegisterStudioJsonBuilder.Build();
        request.Name = string.Empty;
        
        var useCase = CreateUseCase();
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();

        Assert.Single(errors);
        Assert.Contains(ResourceMessagesException.NAME_EMPTY, errors);
    }


    private static RegisterStudioUseCase CreateUseCase(string? name = null)
    {
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var studioRepositoryBuilder = new StudioRepositoryBuilder();
        var studioRepository = studioRepositoryBuilder.Build();

        if (name.NotEmpty())
            studioRepositoryBuilder.ExistsActiveStudioWithName(name);
        
        return new RegisterStudioUseCase(mapper,studioRepository, unitOfWork);

    }
}