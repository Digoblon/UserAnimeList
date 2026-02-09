using System.Net;
using System.Text.Json;
using CommonTestUtilities.Tokens;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Enums;
using WebApi.Test.InlineData;

namespace WebApi.Test.AnimeList.List.me;

public class ListAnimeListTest : UserAnimeListClassFixture
{
    private const string Method = "animelist/me/list";
    
    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;
    private readonly Guid _animeListId;
    private readonly string _animeName;
    
    public ListAnimeListTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _userRole = factory.GetRole();
        _animeListId = factory.GetAnimeListId();
        _animeName = factory.GetAnimeName();
    }
    
    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion,_userRole);
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
    public async Task Error_List_Not_Found(string culture)
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