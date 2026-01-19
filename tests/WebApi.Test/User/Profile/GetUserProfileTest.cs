using System.Net;
using System.Text.Json;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;

namespace WebApi.Test.User.Profile;

public class GetUserProfileTest : UserAnimeListClassFixture
{
    private const string METHOD = "user";
    
    private readonly string _email;
    private readonly string _userName;
    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;


    public GetUserProfileTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _email = factory.GetEmail();
        _userName = factory.GetUserName();
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _userRole = factory.GetRole();
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);
        
        var response = await DoGet(METHOD, token: token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);
        
        Assert.NotNull(responseData.RootElement.GetProperty("userName").GetString());
        Assert.Equal(_userName,responseData.RootElement.GetProperty("userName").GetString());
        Assert.NotNull(responseData.RootElement.GetProperty("email").GetString());
        Assert.Equal(_email,responseData.RootElement.GetProperty("email").GetString());
    }
    
}