using System.Globalization;
using System.Net;
using System.Text.Json;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.Anime.Get.ById;

public class GetAnimeByIdTest : UserAnimeListClassFixture
{
    private const string Method = "anime";
    
    private readonly Guid _animeId;
    private readonly string _animeName;
    
    public GetAnimeByIdTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _animeId = factory.GetAnimeId();
        _animeName = factory.GetAnimeName();
    }
    
    [Fact]
    public async Task Success()
    {
        var response = await DoGet($"{Method}/{_animeId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.Equal(_animeId.ToString(),responseData.RootElement.GetProperty("id").GetString());
        Assert.Equal(_animeName,responseData.RootElement.GetProperty("name").GetString());
    }

    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Anime_Not_Found(string culture)
    {
        var id = Guid.NewGuid();
        
        var response = await DoGet(method:$"{Method}/{id}",culture:culture);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        
        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("ANIME_NOT_FOUND",new  CultureInfo(culture));
        
        Assert.Single(errors);
        Assert.Equal(expectedMessage,errors.First().GetString());
    }
}