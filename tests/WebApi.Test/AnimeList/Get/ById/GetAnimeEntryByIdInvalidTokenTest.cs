using System.Net;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;

namespace WebApi.Test.AnimeList.Get.ById;

public class GetAnimeEntryByIdInvalidTokenTest : UserAnimeListClassFixture
{
    private readonly string METHOD = "animelist";
    
    private readonly Guid _id;
    private readonly UserRole _userRole;

    public GetAnimeEntryByIdInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _userRole = factory.GetRole();
    }
    
    [Fact]
    public async Task Error_Token_Invalid()
    {
        var response = await DoGet($"{METHOD}/{_id}", token: "tokenInvalid");
        
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var response = await DoGet($"{METHOD}/{_id}", token: string.Empty);
        
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
    
    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid(), 1,_userRole);
        
        var response = await DoGet($"{METHOD}/{_id}", token: token);
        
        Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_Version_Mismatch()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, 0, _userRole);
        
        var response = await DoGet($"{METHOD}/{_id}", token: token);
        
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
}