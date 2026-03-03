using CommonTestUtilities.Entities;
using CommonTestUtilities.FileStorage;
using CommonTestUtilities.InlineDatas;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Microsoft.AspNetCore.Http;
using UserAnimeList.Application.UseCases.Anime.Image.Update;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.Anime.Image.Update;

public class UpdateAnimeImageUseCaseTest
{
    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Success(IFormFile file)
    {
        var anime = AnimeBuilder.Build();
        anime.ImagePath = null;
        var request = RequestUpdateImageFormDataBuilder.Build(file);

        var useCase = CreateUseCase(anime);

        var response = await useCase.Execute(request, anime.Id.ToString());
        
        Assert.NotNull(response.ImageUrl);
        Assert.NotEmpty(response.ImageUrl);
    }
    
    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Error_Image_Null(IFormFile file)
    {
        var anime = AnimeBuilder.Build();
        var request = RequestUpdateImageFormDataBuilder.Build(file);
        request.Image = null;

        var useCase = CreateUseCase(anime);
        
        Func<Task> act = async () => await useCase.Execute(request,Guid.NewGuid().ToString());

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.IMAGE_NULL, exception.GetErrorMessages().FirstOrDefault());
    }
    
    [Fact]
    public async Task Error_Image_Exceeds_File_Size()
    {
        var anime = AnimeBuilder.Build();
        var request = RequestUpdateImageFormDataBuilder.Build();
        request.Image = FormFileBuilder.FileExceedsSize();

        var useCase = CreateUseCase(anime);
        
        Func<Task> act = async () => await useCase.Execute(request,Guid.NewGuid().ToString());

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.IMAGE_EXCEEDS_FILE_SIZE, exception.GetErrorMessages().FirstOrDefault());
    }
    
    [Fact]
    public async Task Error_Image_Invalid_Format()
    {
        var anime = AnimeBuilder.Build();
        var request = RequestUpdateImageFormDataBuilder.Build();
        request.Image = FormFileBuilder.Txt();

        var useCase = CreateUseCase(anime);
        
        Func<Task> act = async () => await useCase.Execute(request,Guid.NewGuid().ToString());

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.ONLY_IMAGES_ACCEPTED, exception.GetErrorMessages().FirstOrDefault());
    }
    
    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Error_Anime_Not_Found(IFormFile file)
    {
        var anime = AnimeBuilder.Build();
        var request = RequestUpdateImageFormDataBuilder.Build(file);

        var useCase = CreateUseCase(anime);
        
        Func<Task> act = async () => await useCase.Execute(request,Guid.NewGuid().ToString());

        var exception = await Assert.ThrowsAsync<NotFoundException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.ANIME_NOT_FOUND, exception.GetErrorMessages().FirstOrDefault());
    }
    
    
    
    private static UpdateAnimeImageUseCase CreateUseCase(UserAnimeList.Domain.Entities.Anime anime)
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var animeRepositoryBuilder = new AnimeRepositoryBuilder();
        var animeRepository = animeRepositoryBuilder.GetById(anime).Build();
        var fileStorageBuilder = new FileStorageBuilder();
        var fileStorage = fileStorageBuilder.Save().Build();
        
        return new UpdateAnimeImageUseCase(animeRepository,fileStorage,unitOfWork);

    }
}