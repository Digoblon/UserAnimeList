using System.Net;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;

namespace WebApi.Test.Studio.Delete.SoftDelete;

public class StudioSoftDeleteInvalidTokenTest : UserAnimeListClassFixture
{
    private const string Method = "studio";
    private readonly Guid _id;
    private readonly Guid _studioId;
    
    private readonly UserRole _userRole;
    
    public StudioSoftDeleteInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _userRole = factory.GetRole();
        _studioId = factory.GetStudioId();
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var response = await DoDelete($"{Method}/{_studioId}", token: "tokenInvalid");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid(), 1, _userRole);
        
        var response = await DoDelete($"{Method}/{_studioId}",token: token);
        
        Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_Version_Mismatch()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, 0, _userRole);
        
        var response = await DoDelete($"{Method}/{_studioId}", token: token);
        
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
}