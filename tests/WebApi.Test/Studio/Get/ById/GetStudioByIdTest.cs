using System.Globalization;
using System.Net;
using System.Text.Json;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.Studio.Get.ById;

public class GetStudioByIdTest : UserAnimeListClassFixture
{
    private const string Method = "studio";
    
    private readonly Guid _studioId;
    private readonly string _studioName;
    
    public GetStudioByIdTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _studioId = factory.GetStudioId();
        _studioName = factory.GetStudioName();
    }
    
    [Fact]
    public async Task Success()
    {
        var response = await DoGet($"{Method}/{_studioId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.Equal(_studioId.ToString(),responseData.RootElement.GetProperty("id").GetString());
        Assert.Equal(_studioName,responseData.RootElement.GetProperty("name").GetString());
    }

    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Studio_Not_Found(string culture)
    {
        var id = Guid.NewGuid();
        
        var response = await DoGet(method:$"{Method}/{id}",culture:culture);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        
        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("STUDIO_NOT_FOUND",new  CultureInfo(culture));
        
        Assert.Single(errors);
        Assert.Equal(expectedMessage,errors.First().GetString());
    }
}
