using CommonTestUtilities.Requests;
using UserAnimeList.Application.UseCases.User.Register;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;

namespace Validators.Test.User.Register;

public class RegisterUserValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new RegisterUserValidator();

        var request = RequestRegisterUserJsonBuilder.Build();

        var result = validator.Validate(request);
        
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        //result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Error_UserName_Empty()
    {
        var validator = new RegisterUserValidator();

        var request = RequestRegisterUserJsonBuilder.Build();
        request.UserName = String.Empty;

        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        //result.IsValid.Should().BeFalse();
        Assert.Single(result.Errors);
        Assert.Contains(ResourceMessagesException.NAME_EMPTY, result.Errors.First().ErrorMessage);
        //result.Errors.Should().ContainSingle().And.Contain(e=> e.ErrorMessage.Equals(ResourceMessagesException.NAME_EMPTY));
    }
    
    [Fact]
    public void Error_Email_Empty()
    {
        var validator = new RegisterUserValidator();

        var request = RequestRegisterUserJsonBuilder.Build();
        request.Email = String.Empty;

        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        //result.IsValid.Should().BeFalse();
        Assert.Contains(ResourceMessagesException.EMAIL_EMPTY, result.Errors.First().ErrorMessage);
        //result.Errors.Should().ContainSingle().And.Contain(e=> e.ErrorMessage.Equals(ResourceMessagesException.EMAIL_EMPTY));
    }
    
    [Fact]
    public void Error_Email_Invalid()
    {
        var validator = new RegisterUserValidator();

        var request = RequestRegisterUserJsonBuilder.Build();
        request.Email = "email.com";

        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        //result.IsValid.Should().BeFalse();
        Assert.Contains(ResourceMessagesException.EMAIL_INVALID, result.Errors.First().ErrorMessage);
        //result.Errors.Should().ContainSingle().And.Contain(e=> e.ErrorMessage.Equals(ResourceMessagesException.EMAIL_INVALID));
    }
    
    [Fact]
    public void Error_Multiple_Fields_Empty()
    {
        var validator = new RegisterUserValidator();

        var request = new RequestRegisterUserJson
        {
            UserName = "",
            Email = "",
            Password = ""
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count);

        var messages = result.Errors.Select(e => e.ErrorMessage).ToList();

        Assert.Contains(ResourceMessagesException.NAME_EMPTY, messages);
        Assert.Contains(ResourceMessagesException.EMAIL_EMPTY, messages);
        Assert.Contains(ResourceMessagesException.PASSWORD_EMPTY, messages);
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
        var validator = new RegisterUserValidator();

        var request = RequestRegisterUserJsonBuilder.Build(passwordLength);

        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        //result.IsValid.Should().BeFalse();
        Assert.Contains(ResourceMessagesException.INVALID_PASSWORD, result.Errors.First().ErrorMessage);
        //result.Errors.Should().ContainSingle().And.Contain(e=> e.ErrorMessage.Equals(ResourceMessagesException.INVALID_PASSWORD));
        Assert.NotInRange(request.Password.Length, 8, 50);
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
        var validator = new RegisterUserValidator();

        var request = RequestRegisterUserJsonBuilder.Build();
        request.Password = password;

        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        //result.IsValid.Should().BeFalse();
        Assert.Contains(ResourceMessagesException.INVALID_PASSWORD, result.Errors.First().ErrorMessage);
        //result.Errors.Should().ContainSingle().And.Contain(e=> e.ErrorMessage.Equals(ResourceMessagesException.INVALID_PASSWORD));
        Assert.InRange(request.Password.Length, 8, 50);
    }
    
    
    [Fact]
    public void Error_Password_Empty()
    {
        var validator = new RegisterUserValidator();

        var request = RequestRegisterUserJsonBuilder.Build();
        request.Password = string.Empty;

        var result = validator.Validate(request);
        
        Assert.False(result.IsValid);
        //result.IsValid.Should().BeFalse();
        Assert.Contains(ResourceMessagesException.PASSWORD_EMPTY, result.Errors.First().ErrorMessage);
        //result.Errors.Should().ContainSingle().And.Contain(e=> e.ErrorMessage.Equals(ResourceMessagesException.PASSWORD_EMPTY));
    }

}