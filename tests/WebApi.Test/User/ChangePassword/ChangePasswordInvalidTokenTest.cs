using System.Net;
using CommonTestUtilities.Tokens;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Enums;

namespace WebApi.Test.User.ChangePassword;

public class ChangePasswordInvalidTokenTest : UserAnimeListClassFixture
{
    private const string METHOD = "user/change-password";

    private readonly Guid _id;
    private readonly UserRole _userRole;
    public ChangePasswordInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _userRole = factory.GetRole();
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
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid(), 1,_userRole);
        var request = new RequestChangePasswordJson();
        var response = await DoPut(METHOD,request:request, token: token);
        
        Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_Version_Mismatch()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, 0,_userRole);
        var request = new RequestChangePasswordJson();
        var response = await DoPut(METHOD,request:request, token: token);
        
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
}