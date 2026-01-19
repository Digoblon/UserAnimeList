using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Enums;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.ChangePassword;

public class ChangePasswordTest : UserAnimeListClassFixture
{
    private const string METHOD = "user/change-password";

    private readonly Guid _id;
    private readonly string _email;
    private readonly string _password;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;

    public ChangePasswordTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _password = factory.GetPassword();
        _email = factory.GetEmail();
        _userRole = factory.GetRole();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        request.Password = _password;
        
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion,_userRole);
        
        var response = await DoPut(METHOD, request, token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.NotNull(responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString());
        Assert.NotEmpty(responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString()!);

        var loginRequest = new RequestLoginJson
        {
            Login = _email,
            Password = _password
        };
        
        response = await DoPost(method:"login", request:loginRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        loginRequest.Password = request.NewPassword;
        
        response = await DoPost(method:"login", request:loginRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_NewPassword_Empty(string culture)
    {
        var request = new RequestChangePasswordJson
        {
            Password = _password,
            NewPassword = string.Empty,
            ConfirmNewPassword = string.Empty
        };
        
        
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);
        
        var response = await DoPut(METHOD, request,token,culture);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);
        
        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("PASSWORD_EMPTY", new CultureInfo(culture));
        
        Assert.Single(errors);
        Assert.Equal(expectedMessage,errors.First().GetString());
    }
}