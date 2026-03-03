using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.AnimeList.Get.ById;

public class GetAnimeEntryByIdTest : UserAnimeListClassFixture
{
    private const string Method = "animelist";
    
    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;
    private readonly Guid _animeListId;
    
    public GetAnimeEntryByIdTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _userRole = factory.GetRole();
        _animeListId = factory.GetAnimeListId();
    }
    
    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion,_userRole);
        
        var response = await DoGet($"{Method}/{_animeListId}",token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.Equal(_animeListId.ToString(),responseData.RootElement.GetProperty("id").GetString());
    }
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_List_Not_Found(string culture)
    {
        var id = Guid.NewGuid();
        
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);
        
        var response = await DoGet(method:$"{Method}/{id}", token,culture:culture);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        
        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("ANIME_LIST_INVALID",new  CultureInfo(culture));
        
        Assert.Single(errors);
        Assert.Equal(expectedMessage,errors.First().GetString());
    }
}