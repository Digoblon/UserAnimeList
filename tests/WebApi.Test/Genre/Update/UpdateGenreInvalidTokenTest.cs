using System.Net;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;

namespace WebApi.Test.Genre.Update;

public class UpdateGenreInvalidTokenTest : UserAnimeListClassFixture
{
    private const string Method = "genre";
    private readonly Guid _id;
    private readonly Guid _genreId;
    
    private readonly UserRole _userRole;
    
    public UpdateGenreInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _genreId = factory.GetGenreId();
        _userRole = factory.GetRole();
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestUpdateGenreJsonBuilder.Build();
        var response = await DoPut($"{Method}/{_genreId}",request, token: "tokenInvalid");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid(), 1, _userRole);
        var request = RequestUpdateGenreJsonBuilder.Build();
        var response = await DoPut($"{Method}/{_genreId}",request,token: token);
        
        Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_Version_Mismatch()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, 0, _userRole);
        var request = RequestUpdateGenreJsonBuilder.Build();
        var response = await DoPut($"{Method}/{_genreId}",request ,token: token);
        
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
}