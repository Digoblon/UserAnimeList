using System.Net;
using CommonTestUtilities.Tokens;

namespace WebApi.Test.User.Delete.SoftDelete;

public class SoftDeleteTest : UserAnimeListClassFixture
{
    private const string METHOD = "user/me";

    private readonly Guid _id;
    private readonly int _tokenVersion;


    public SoftDeleteTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion);
        
        var response = await DoDelete(METHOD,token);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

}