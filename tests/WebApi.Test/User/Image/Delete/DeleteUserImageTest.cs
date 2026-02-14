using System.Net;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;

namespace WebApi.Test.User.Image.Delete;

public class DeleteUserImageTest : UserAnimeListClassFixture
{
    private const string Method = "user/me/image";

    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;


    public DeleteUserImageTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _userRole = factory.GetRole();
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);
        
        var response = await DoDelete(Method, token);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}