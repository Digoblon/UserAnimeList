using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.Studio.Register;
using UserAnimeList.Application.UseCases.User.Register;
using UserAnimeList.Exception;

namespace Validators.Test.Studio.Register;

public class RegisterStudioValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new RegisterStudioValidator();

        var request = RequestRegisterStudioJsonBuilder.Build();

        var result = validator.Validate(request);
        
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
    
    [Fact]
    public void Error_UserName_Empty()
    {
        var validator = new RegisterStudioValidator();

        var request = RequestRegisterStudioJsonBuilder.Build();
        request.Name = String.Empty;

        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(ResourceMessagesException.NAME_EMPTY, result.Errors.First().ErrorMessage);
    }
}