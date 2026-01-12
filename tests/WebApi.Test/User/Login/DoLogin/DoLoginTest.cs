using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.Login.DoLogin;

public class DoLoginTest : UserAnimeListClassFixture
{
    private readonly string method = "login";
    
    private readonly string _email;
    private readonly string _password;
    private readonly string _userName;

    public DoLoginTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _email = factory.GetEmail();
        _password = factory.GetPassword();
        _userName = factory.GetUserName();
    }
    
    
    [Fact]
    public async Task Success_Email()
    {
        var request = new RequestLoginJson()
        {
            Login = _email,
            Password = _password
        };
        
        var response = await DoPost(method:method, request:request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);
        
        Assert.NotNull(responseData.RootElement.GetProperty("userName").GetString());
        Assert.NotEmpty(responseData.RootElement.GetProperty("userName").GetString()!);
        Assert.Equal(responseData.RootElement.GetProperty("userName").GetString(), _userName);
        Assert.NotNull(responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString());
        Assert.NotEmpty(responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString()!);
    }
    
    

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Login_Invalid(string culture)
    {
        var request = RequestLoginJsonBuilder.Build();
        
        var response = await DoPost(method:method, request:request,culture:culture);
        //response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        
        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("EMAIL_OR_PASSWORD_INVALID", new CultureInfo(culture));
        
        //errors.Should().ContainSingle().And.Contain(error => error.GetString()!.Equals(expectedMessage));
    }
    
}