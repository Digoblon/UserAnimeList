using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.Genre.Register;
using UserAnimeList.Domain.Extensions;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Genre.Register;

public class RegisterGenreUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterGenreJsonBuilder.Build();

        var useCase = CreateUseCase();
        
        var result = await useCase.Execute(request);

        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
    }
    
    [Fact]
    public async Task Error_Name_Already_Registered()
    {
        var request = RequestRegisterGenreJsonBuilder.Build();
        
        var useCase = CreateUseCase(request.Name);
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();

        Assert.Single(errors);
        Assert.Contains(ResourceMessagesException.GENRE_ALREADY_REGISTERED, errors);
    }
    
    [Fact]
    public async Task Error_Name_Empty()
    {
        var request = RequestRegisterGenreJsonBuilder.Build();
        request.Name = string.Empty;
        
        var useCase = CreateUseCase();
        
        Func<Task> act = async ()  => await useCase.Execute(request);
        
        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);

        var errors = exception.GetErrorMessages();

        Assert.Single(errors);
        Assert.Contains(ResourceMessagesException.NAME_EMPTY, errors);
    }


    private static RegisterGenreUseCase CreateUseCase(string? name = null)
    {
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var genreRepositoryBuilder = new GenreRepositoryBuilder();
        var genreRepository = genreRepositoryBuilder.Build();

        if (name.NotEmpty())
            genreRepositoryBuilder.ExistsActiveGenreWithName(name);
        
        return new RegisterGenreUseCase(mapper,genreRepository, unitOfWork);

    }
}