using System.Net;
using System.Text.Json;
using UserAnimeList.Communication.Requests;

namespace WebApi.Test.Genre.Get.ByName;

public class GetGenreByNameTest : UserAnimeListClassFixture
{
    private const string Method = "genre/search";
    
    private readonly string _genreName;
    
    public GetGenreByNameTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _genreName = factory.GetGenreName();
    }
    
    [Fact]
    public async Task Success()
    {
        var request = new RequestGenreGetByNameJson
        {
            Name = _genreName
        };
        var response = await DoPost(Method,request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        Assert.NotEmpty(responseData.RootElement.GetProperty("genres").EnumerateArray());
    }

    
    [Fact]
    public async Task Success_NoContent()
    {
        var request = new RequestGenreGetByNameJson
        {
            Name = "aaaaaaaa"
        };
        
        var response = await DoPost(method:Method, request:request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}