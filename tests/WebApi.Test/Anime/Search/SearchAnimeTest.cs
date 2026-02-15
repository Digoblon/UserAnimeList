using System.Net;
using System.Text.Json;
using UserAnimeList.Communication.Requests;
using WebApi.Test.InlineData;

namespace WebApi.Test.Anime.Search;

public class SearchAnimeTest : UserAnimeListClassFixture
{
    private const string Method = "anime/search";

    private readonly string _animeName;
    private readonly Guid _animeId;
    
    public SearchAnimeTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _animeName = factory.GetAnimeName();
        _animeId = factory.GetAnimeId();
    }
    
    [Fact]
    public async Task Success()
    {
        var request = new RequestAnimeSearchJson
        {
            Query = _animeName
        };
        var response = await DoGetQuery($"{Method}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.NotEmpty(responseData.RootElement.GetProperty("animes").EnumerateArray());
    }

    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Success_Empty_Query(string culture)
    {
        var request = new RequestAnimeSearchJson
        {
            Query = string.Empty
        };
        var response = await DoGetQuery($"{Method}", request, culture: culture);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

}
