using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Communication.Enums;
using UserAnimeList.Enums;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.AnimeList.Update;

public class UpdateAnimeEntryTest : UserAnimeListClassFixture
{
    private readonly string method = "animelist";
    
    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;
    private readonly Guid _animeId;
    private readonly Guid _animeListId;
    
    public UpdateAnimeEntryTest(CustomWebApplicationFactory factory) : base(factory)
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
        var request = RequestUpdateAnimeListEntryJsonBuilder.Build();

        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);

        var response = await DoPut(method: $"{method}/{_animeListId}", request: request, token: token);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Status_Invalid(string culture)
    {
        var request = RequestUpdateAnimeListEntryJsonBuilder.Build();
        request.Status = (AnimeEntryStatus)100;

        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);

        var response = await DoPut(method: $"{method}/{_animeListId}", request: request, token: token, culture: culture);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage =
            ResourceMessagesException.ResourceManager.GetString("ANIME_LIST_INVALID_STATUS", new CultureInfo(culture));

        Assert.Single(errors);
        Assert.Equal(expectedMessage, errors.First().ToString());
    }
}