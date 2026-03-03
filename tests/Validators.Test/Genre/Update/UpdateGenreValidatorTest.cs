using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.Genre.Update;
using UserAnimeList.Exception;

namespace Validators.Test.Genre.Update;

public class UpdateGenreValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateGenreValidator();

        var request = RequestUpdateGenreJsonBuilder.Build();

        var result = validator.Validate(request);
        
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
    
    [Fact]
    public void Error_UserName_Empty()
    {
        var validator = new UpdateGenreValidator();

        var request = RequestUpdateGenreJsonBuilder.Build();
        request.Name = String.Empty;

        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(ResourceMessagesException.NAME_EMPTY, result.Errors.First().ErrorMessage);
    }
}