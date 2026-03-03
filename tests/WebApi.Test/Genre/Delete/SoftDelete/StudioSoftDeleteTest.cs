using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

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
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Genre_Not_Found(string culture)
    {
        var id = Guid.NewGuid();
        
        var response = await DoGet(method:$"{Method}/{id}",culture:culture);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        
        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("GENRE_NOT_FOUND",new  CultureInfo(culture));
        
        Assert.Single(errors);
        Assert.Equal(expectedMessage,errors.First().GetString());
    }
}