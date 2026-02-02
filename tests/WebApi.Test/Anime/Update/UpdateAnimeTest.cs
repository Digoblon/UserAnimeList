using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.Anime.Update;

public class UpdateAnimeTest : UserAnimeListClassFixture
{
    private const string Method = "anime";
    
    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;
    private readonly Guid _genreId;
    private readonly Guid _studioId;
    private readonly Guid _animeId;
    
    
    public UpdateAnimeTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _userRole = factory.GetRole();
        _genreId = factory.GetGenreId();
        _studioId = factory.GetStudioId();
        _animeId = factory.GetAnimeId();
    }
    
    [Fact]
    public async Task Success()
    {
        var request = RequestAnimeJsonBuilder.Build();
        request.Genres.Clear();
        request.Genres.Add(_genreId);
        request.Studios.Clear();
        request.Studios.Add(_studioId);

        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);

        var response = await DoPut($"{Method}/{_animeId}", request, token);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Title_Empty(string culture)
    {
        var request = RequestAnimeJsonBuilder.Build();
        request.Genres.Clear();
        request.Genres.Add(_genreId);
        request.Studios.Clear();
        request.Studios.Add(_studioId);
        request.Name = string.Empty;

        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);

        var response = await DoPut($"{Method}/{_animeId}", request, token, culture);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

        Assert.Single(errors);
        Assert.Equal(expectedMessage,errors.First().GetString());
    }
}