using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.Genre.Register;
using UserAnimeList.Exception;

namespace Validators.Test.Genre.Register;

public class RegisterGenreValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new RegisterGenreValidator();

        var request = RequestRegisterGenreJsonBuilder.Build();

        var result = validator.Validate(request);
        
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
    
    [Fact]
    public void Error_UserName_Empty()
    {
        var validator = new RegisterGenreValidator();

        var request = RequestRegisterGenreJsonBuilder.Build();
        request.Name = String.Empty;

        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(ResourceMessagesException.NAME_EMPTY, result.Errors.First().ErrorMessage);
    }
}