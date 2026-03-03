using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.User.ChangePassword;
using UserAnimeList.Exception;

namespace Validators.Test.User.ChangePassword;

public class ChangePasswordValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Build();
        
        var result = validator.Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(51)]
    [InlineData(52)]
    public void Error_Password_Length_Invalid(int passwordLength)
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Build(passwordLength);

        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.INVALID_PASSWORD,  result.Errors.First().ErrorMessage);
    }
    
    [Theory]
    [InlineData("aaaaaaaaaa")]
    [InlineData("Aaaaaaaaaa")]
    [InlineData("AAAAAAAAAA")]
    [InlineData("1111111111")]
    [InlineData("1aaaaaaaaa")]
    [InlineData("1AAAAAAAAA")]
    public void Error_Password_Strength_Invalid(string password)
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Build();
        request.NewPassword = password;
        request.ConfirmNewPassword = password;

        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.INVALID_PASSWORD,  result.Errors.First().ErrorMessage);
    }
    
    [Fact]
    public void Error_Password_Empty()
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Build();
        request.Password = string.Empty;
        request.NewPassword = string.Empty;

        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ResourceMessagesException.PASSWORD_EMPTY,  result.Errors.First().ErrorMessage);
    }
}