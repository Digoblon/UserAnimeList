using System.Net;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;

namespace WebApi.Test.Studio.Update;

public class UpdateStudioInvalidTokenTest : UserAnimeListClassFixture
{
    private const string Method = "studio";
    private readonly Guid _id;
    private readonly Guid _studioId;
    
    private readonly UserRole _userRole;
    
    public UpdateStudioInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _studioId = factory.GetStudioId();
        _userRole = factory.GetRole();
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestUpdateStudioJsonBuilder.Build();
        var response = await DoPut($"{Method}/{_studioId}",request, token: "tokenInvalid");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid(), 1, _userRole);
        var request = RequestUpdateStudioJsonBuilder.Build();
        var response = await DoPut($"{Method}/{_studioId}",request,token: token);
        
        Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_Version_Mismatch()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, 0, _userRole);
        var request = RequestUpdateStudioJsonBuilder.Build();
        var response = await DoPut($"{Method}/{_studioId}",request ,token: token);
        
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
}