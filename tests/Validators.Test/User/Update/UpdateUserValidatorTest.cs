using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.User.Update;
using UserAnimeList.Exception;

namespace Validators.Test.User.Update;

public class UpdateUserValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateUserValidator();

        var request = RequestUpdateUserJsonBuilder.Build();
        
        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Error_UserName_Empty()
    {
        var validator = new UpdateUserValidator();

        var request = RequestUpdateUserJsonBuilder.Build();
        request.UserName = string.Empty;
        
        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.NAME_EMPTY,result.Errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_UserName_Invalid()
    {
        var validator = new UpdateUserValidator();

        var request = RequestUpdateUserJsonBuilder.Build();
        request.UserName = request.UserName + "@";
        
        var result = validator.Validate(request);

        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.NAME_INVALID_FORMAT,result.Errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_Email_Empty()
    {
        var validator = new UpdateUserValidator();

        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = string.Empty;
        
        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.EMAIL_EMPTY,result.Errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_Email_Invalid()
    {
        var validator = new UpdateUserValidator();

        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = "email.com";
        
        var result = validator.Validate(request);

        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.EMAIL_INVALID,result.Errors.First().ErrorMessage);
    }
}