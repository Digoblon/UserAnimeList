using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.Anime.Register;

public class RegisterAnimeTest : UserAnimeListClassFixture
{
    private readonly string method = "anime";
    
    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;
    private readonly Guid _studioId;
    private readonly Guid _genreId;
    
    public RegisterAnimeTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _userRole = factory.GetRole();
        _studioId = factory.GetStudioId();
        _genreId = factory.GetGenreId();
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

        var response = await DoPost(method: method, request: request, token: token);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.NotNull(responseData.RootElement.GetProperty("name").GetString());
        Assert.NotEmpty(responseData.RootElement.GetProperty("name").GetString()!);
        Assert.Equal(request.Name, responseData.RootElement.GetProperty("name").GetString());
    }
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Empty_Name(string culture)
    {
        var request = RequestAnimeJsonBuilder.Build();
        request.Genres.Clear();
        request.Genres.Add(_genreId);
        request.Studios.Clear();
        request.Studios.Add(_studioId);
        request.Name = string.Empty;

        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);

        var response = await DoPost(method: method, request: request, token: token);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage =
            ResourceMessagesException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

        Assert.Single(errors);
        Assert.Equal(expectedMessage, errors.First().ToString());
    }
}