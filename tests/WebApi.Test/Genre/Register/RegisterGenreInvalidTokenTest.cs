using System.Net;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;

namespace WebApi.Test.Genre.Register;

public class RegisterGenreInvalidTokenTest : UserAnimeListClassFixture
{
    private const string Method = "genre";
    private readonly Guid _id;
    
    private readonly UserRole _userRole;
    
    public RegisterGenreInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _userRole = factory.GetRole();
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestRegisterGenreJsonBuilder.Build();
        var response = await DoPost(Method,request, token: "tokenInvalid");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid(), 1, _userRole);
        var request = RequestRegisterGenreJsonBuilder.Build();
        var response = await DoPost(Method,request,token: token);
        
        Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);
    }

    [Fact]
    public async Task Error_Token_Version_Mismatch()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, 0, _userRole);
        var request = RequestRegisterGenreJsonBuilder.Build();
        var response = await DoPost(Method,request ,token: token);
        
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
}