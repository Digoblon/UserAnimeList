using System.Net;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;

namespace WebApi.Test.Studio.Delete.SoftDelete;

public class StudioSoftDeleteTest : UserAnimeListClassFixture
{
    private const string Method = "studio";
    
    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;
    
    private readonly Guid _studioId;
    
    public StudioSoftDeleteTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _userRole = factory.GetRole();
        
        _studioId = factory.GetStudioId();
    }
    
    [Fact]
    public async Task Success()
    {
        
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion,_userRole);
        
        var response = await DoDelete($"{Method}/{_studioId}",token);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}