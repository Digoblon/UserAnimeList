using System.Globalization;
using System.Net;
using System.Text.Json;
using UserAnimeList.Communication.Requests;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.Token.UseRefreshToken;

public class GetNewAccessTokenTest : UserAnimeListClassFixture
{
    private const string METHOD = "token";

    private readonly string _userRefreshToken;

    public GetNewAccessTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userRefreshToken = factory.GetRefreshToken();
    }

    [Fact]
    public async Task Success()
    {
        var request = new RequestNewTokenJson
        {
            RefreshToken = _userRefreshToken
        };

        var response = await DoPost($"{METHOD}/refresh-token", request);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);
        
        Assert.NotNull(responseData.RootElement.GetProperty("accessToken").GetString());
        Assert.NotEmpty(responseData.RootElement.GetProperty("accessToken").GetString()!);
        Assert.NotNull(responseData.RootElement.GetProperty("refreshToken").GetString());
        Assert.NotEmpty(responseData.RootElement.GetProperty("refreshToken").GetString()!);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Login_Invalid(string culture)
    {
        var request = new RequestNewTokenJson
        {
            RefreshToken = "InvalidRefreshToken"
        };

        var response = await DoPost($"{METHOD}/refresh-token", request, culture: culture);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("EXPIRED_SESSION", new CultureInfo(culture));

        Assert.Single(errors);
        Assert.Equal(expectedMessage,errors.First().GetString());
    }
}