using System.Net;
using CommonTestUtilities.Tokens;

namespace WebApi.Test.User.Delete.SoftDelete;

public class SoftDeleteInvalidTokenTest : UserAnimeListClassFixture
{
    private const string METHOD = "user/me";
    private readonly Guid _id;
    
    public SoftDeleteInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var response = await DoDelete(METHOD, token: "tokenInvalid");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid(), 1);
        
        var response = await DoDelete(METHOD,token: token);
        
        Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_Version_Mismatch()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, 0);
        
        var response = await DoDelete(METHOD, token: token);
        
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
}