using System.Net;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;

namespace WebApi.Test.Anime.Image.Update;

public class UpdateAnimeImageInvalidTokenTest : UserAnimeListClassFixture
{
    private const string Method = "anime";
    
    private readonly Guid _id;
    private readonly UserRole _userRole;
    private readonly Guid _animeId;
    
    public UpdateAnimeImageInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _userRole = factory.GetRole();
        _animeId = factory.GetAnimeId();
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var response = await DoPut($"{Method}/{_animeId}/image", request, token: "tokenInvalid");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid(), 1,_userRole);
        
        var request = RequestUpdateUserJsonBuilder.Build();
        
        var response = await DoPut($"{Method}/{_animeId}/image",request, token: token);
        
        Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_Version_Mismatch()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, 0,_userRole);
        
        var request = RequestUpdateUserJsonBuilder.Build();
        
        var response = await DoPut($"{Method}/{_animeId}/image",request:request, token: token);
        
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
}