using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Enums;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.AnimeList.List.me;

public class ListAnimeListTest : UserAnimeListClassFixture
{
    private const string Method = "animelist/me/list";
    
    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;
    private readonly Guid _animeId;
    private readonly Guid _animeListId;
    private readonly string _animeName;
    
    public ListAnimeListTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _userRole = factory.GetRole();
        _animeId = factory.GetAnimeId();
        _animeListId = factory.GetAnimeListId();
        _animeName = factory.GetAnimeName();
    }
    
    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion,_userRole);

        await DoDelete($"animelist/{_animeListId}", token);

        var addRequest = RequestAnimeListEntryJsonBuilder.Build();
        addRequest.AnimeId = _animeId;

        var addResponse = await DoPost("animelist", addRequest, token);
        Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);

        var request = new RequestAnimeListEntryFilterJson
        {
            Query = _animeName
        };
        
        var response = await DoGetQuery(Method,request,token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.NotEmpty(responseData.RootElement.GetProperty("lists").EnumerateArray());
    }
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Query_Exceeded(string culture)
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion,_userRole);
        var request = new RequestAnimeListEntryFilterJson
        {
            Query = new string('a', 257)
        };

        var response = await DoGetQuery(Method,request,token,culture);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);
        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var expectedMessage =
            ResourceMessagesException.ResourceManager.GetString("ANIME_LIST_QUERY_EXCEEDED", new CultureInfo(culture));
        
        Assert.Single(errors);
        Assert.Equal(expectedMessage, errors.First().ToString());
    }
    
    [Fact]
    public async Task Success_Anime_Not_Found_Result_Empty()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion,_userRole);
        var request = new RequestAnimeListEntryFilterJson
        {
            Query = _animeName
        };

        await DoDelete($"animelist/{_animeListId}", token);
        
        var response = await DoGetQuery(Method,request,token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.Empty(responseData.RootElement.GetProperty("lists").EnumerateArray());
    }
}
