using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.Error;

public class ErrorContractTest : UserAnimeListClassFixture
{
    public ErrorContractTest(CustomWebApplicationFactory factory) : base(factory) { }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_BadRequest_Should_Return_Standard_Contract(string culture)
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.UserName = string.Empty;

        var response = await DoPost("user", request, culture: culture);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        var root = responseData.RootElement;

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));
        var errors = root.GetProperty("errors").EnumerateArray().Select(error => error.GetString()).ToList();

        Assert.Single(errors);
        Assert.Equal(expectedMessage, errors.First());
        Assert.Equal(expectedMessage, root.GetProperty("message").GetString());
        Assert.Equal("validation_error", root.GetProperty("code").GetString());
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("traceId").GetString()));
        Assert.False(root.GetProperty("tokenIsExpired").GetBoolean());
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Unauthorized_Without_Token_Should_Return_Standard_Contract(string culture)
    {
        var response = await DoGet("user", token: string.Empty, culture: culture);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        var root = responseData.RootElement;

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("NO_TOKEN", new CultureInfo(culture));
        var errors = root.GetProperty("errors").EnumerateArray().Select(error => error.GetString()).ToList();

        Assert.Single(errors);
        Assert.Equal(expectedMessage, errors.First());
        Assert.Equal(expectedMessage, root.GetProperty("message").GetString());
        Assert.Equal("invalid_token", root.GetProperty("code").GetString());
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("traceId").GetString()));
        Assert.False(root.GetProperty("tokenIsExpired").GetBoolean());
    }
}
