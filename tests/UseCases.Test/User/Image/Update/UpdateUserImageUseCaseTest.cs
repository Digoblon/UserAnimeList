using CommonTestUtilities.Entities;
using CommonTestUtilities.FileStorage;
using CommonTestUtilities.InlineDatas;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Microsoft.AspNetCore.Http;
using UserAnimeList.Application.UseCases.User.Image.Update;
using UserAnimeList.Exception;
using UserAnimeList.Exception.Exceptions;

namespace UseCases.Test.User.Image.Update;

public class UpdateUserImageUseCaseTest
{
    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Success(IFormFile file)
    {
        var (user, _) = UserBuilder.Build();
        user.ImagePath = null;
        var request = RequestUpdateImageFormDataBuilder.Build(file);

        var useCase = CreateUseCase(user);

        var response = await useCase.Execute(request);
        
        Assert.NotNull(response.ImageUrl);
        Assert.NotEmpty(response.ImageUrl);
    }
    
    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task Error_Image_Null(IFormFile file)
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestUpdateImageFormDataBuilder.Build(file);
        request.Image = null;

        var useCase = CreateUseCase(user);
        
        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.IMAGE_NULL, exception.GetErrorMessages().FirstOrDefault());
    }
    
    [Fact]
    public async Task Error_Image_Exceeds_File_Size()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestUpdateImageFormDataBuilder.Build();
        request.Image = FormFileBuilder.FileExceedsSize();
        
        var useCase = CreateUseCase(user);
        
        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.IMAGE_EXCEEDS_FILE_SIZE, exception.GetErrorMessages().FirstOrDefault());
    }
    
    [Fact]
    public async Task Error_Image_Invalid_Format()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestUpdateImageFormDataBuilder.Build();
        request.Image = FormFileBuilder.Txt();

        var useCase = CreateUseCase(user);
        
        Func<Task> act = async () => await useCase.Execute(request);

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(act);
        Assert.Single(exception.GetErrorMessages());
        Assert.Equal(ResourceMessagesException.ONLY_IMAGES_ACCEPTED, exception.GetErrorMessages().FirstOrDefault());
    } 
    
    private static UpdateUserImageUseCase CreateUseCase(UserAnimeList.Domain.Entities.User user)
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var userRepository = new UserRepositoryBuilder().Build();
        var fileStorageBuilder = new FileStorageBuilder();
        var fileStorage = fileStorageBuilder.Save().Build();
        
        return new UpdateUserImageUseCase(loggedUser,userRepository,fileStorage,unitOfWork);

    }
}