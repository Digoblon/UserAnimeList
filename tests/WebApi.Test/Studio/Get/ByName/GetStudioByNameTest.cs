using System.Net;
using System.Text.Json;
using UserAnimeList.Communication.Requests;

namespace WebApi.Test.Studio.Get.ByName;

public class GetStudioByNameTest : UserAnimeListClassFixture
{
    private const string Method = "studio/search";
    
    private readonly string _studioName;
    
    public GetStudioByNameTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _studioName = factory.GetStudioName();
    }
    
    [Fact]
    public async Task Success()
    {
        var request = new RequestStudioGetByName
        {
            Name = _studioName
        };
        var response = await DoPost(Method,request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.NotEmpty(responseData.RootElement.GetProperty("studios").EnumerateArray());
    }

    
    [Fact]
    public async Task Success_NoContent()
    {
        var request = new RequestStudioGetByName
        {
            Name = "aaaaaaaa"
        };
        
        var response = await DoPost(method:Method, request:request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}