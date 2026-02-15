using System.Net;
using System.Text.Json;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Communication.Requests;

namespace WebApi.Test.Anime.Filter;

public class AnimeFilterTest : UserAnimeListClassFixture
{
    private const string Method = "anime/filter";

    private readonly string _animeName;
    private readonly Guid _animeId;
    
    public AnimeFilterTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _animeName = factory.GetAnimeName();
        _animeId = factory.GetAnimeId();
    }
    
    [Fact]
    public async Task Success()
    {
        var request = new RequestAnimeFilterJson
        {
            Query = _animeName
        };

        var response = await DoGetQuery($"{Method}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.NotEmpty(responseData.RootElement.GetProperty("animes").EnumerateArray());
    }
    
    [Fact]
    public async Task Success_No_Content()
    {
        var request = new RequestAnimeFilterJson
        {
            Query = "NoMatchAnime"
        };

        var response = await DoGetQuery($"{Method}", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Error_Status_Invalid()
    {
        var request = new RequestAnimeFilterJson
        {
            Status = (AnimeStatus)100
        };

        var response = await DoGetQuery($"{Method}", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
