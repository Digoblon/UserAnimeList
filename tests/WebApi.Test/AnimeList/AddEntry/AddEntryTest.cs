using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Enums;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.AnimeList.AddEntry;
 
public class AddEntryTest : UserAnimeListClassFixture
{
    private readonly string method = "animelist";
    
    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;
    private readonly Guid _animeId;
    private readonly Guid _animeListId;
    
    public AddEntryTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _userRole = factory.GetRole();
        _animeId = factory.GetAnimeId();
        _animeListId = factory.GetAnimeListId();
    }
    
    [Fact]
    public async Task Success()
    {
        var request = RequestAnimeListEntryJsonBuilder.Build();
        request.AnimeId = _animeId;

        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);
        await DoDelete(method: $"{method}/{_animeListId}", token: token);
        var response = await DoPost(method: method, request: request, token: token);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.NotNull(responseData.RootElement.GetProperty("id").GetString());
        Assert.NotEmpty(responseData.RootElement.GetProperty("id").GetString()!);
        Assert.Equal(request.AnimeId.ToString(), responseData.RootElement.GetProperty("animeId").GetString());
    }
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Anime_Invalid(string culture)
    {
        var request = RequestAnimeListEntryJsonBuilder.Build();

        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);

        await DoDelete(method: $"{method}/{_animeListId}", token: token);
        var response = await DoPost(method: method, request: request, token: token, culture: culture);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage =
            ResourceMessagesException.ResourceManager.GetString("ANIME_NOT_FOUND", new CultureInfo(culture));

        Assert.Single(errors);
        Assert.Equal(expectedMessage, errors.First().ToString());
    }
}
