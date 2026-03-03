using System.Globalization;
using System.Net;
using System.Text.Json;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.Genre.Get.ById;

public class GetGenreByIdTest : UserAnimeListClassFixture
{
    private const string Method = "genre";
    
    private readonly Guid _genreId;
    private readonly string _genreName;
    
    public GetGenreByIdTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _genreId = factory.GetGenreId();
        _genreName = factory.GetGenreName();
    }
    
    [Fact]
    public async Task Success()
    {
        var response = await DoGet($"{Method}/{_genreId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.Equal(_genreId.ToString(),responseData.RootElement.GetProperty("id").GetString());
        Assert.Equal(_genreName,responseData.RootElement.GetProperty("name").GetString());
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
