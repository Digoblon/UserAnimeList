using System.Net;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;

namespace WebApi.Test.Genre.Delete.SoftDelete;

public class GenreSoftDeleteTest : UserAnimeListClassFixture
{
    private const string Method = "genre";
    
    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;
    
    private readonly Guid _genreId;
    
    public GenreSoftDeleteTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _userRole = factory.GetRole();
        
        _genreId = factory.GetGenreId();
    }
    
    [Fact]
    public async Task Success()
    {
        
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion,_userRole);
        
        var response = await DoDelete($"{Method}/{_genreId}",token);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}