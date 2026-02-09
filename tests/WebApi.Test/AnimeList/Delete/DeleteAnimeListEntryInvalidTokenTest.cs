using System.Net;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;

namespace WebApi.Test.AnimeList.Delete;

public class DeleteAnimeListEntryInvalidTokenTest : UserAnimeListClassFixture
{
    private const string Method = "animelist";
    private readonly Guid _id;
    private readonly Guid _animeId;
    
    private readonly UserRole _userRole;
    
    public DeleteAnimeListEntryInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _userRole = factory.GetRole();
        _animeId = factory.GetAnimeId();
    }
    
    [Fact]
    public async Task Error_Token_Invalid()
    {
        var response = await DoDelete($"{Method}/{_animeId}", token: "tokenInvalid");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid(), 1, _userRole);
        var response = await DoDelete($"{Method}/{_animeId}",token: token);
        
        Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_Version_Mismatch()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, 0, _userRole);
        var response = await DoDelete($"{Method}/{_animeId}" ,token: token);
        
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
}