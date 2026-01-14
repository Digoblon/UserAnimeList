using System.Net;
using CommonTestUtilities.Tokens;
using UserAnimeList.Communication.Requests;

namespace WebApi.Test.User.ChangePassword;

public class ChangePasswordInvalidTokenTest : UserAnimeListClassFixture
{
    private const string METHOD = "user/change-password";
    public ChangePasswordInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = new RequestChangePasswordJson();
        
        var response = await DoPut(METHOD, request,token:"tokenInvalid");
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task Error_Without_Token()
    {
        var request = new RequestChangePasswordJson();
        
        var response = await DoPut(METHOD, request,token:string.Empty);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var request = new RequestChangePasswordJson();

        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());
        
        var response = await DoPut(METHOD, request,token);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}