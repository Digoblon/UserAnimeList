using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.Studio.Register;

public class RegisterStudioTest : UserAnimeListClassFixture
{
    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;

    private readonly string method = "studio";

    public RegisterStudioTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _userRole = factory.GetRole();
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterStudioJsonBuilder.Build();


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
        var request = RequestRegisterStudioJsonBuilder.Build();
        request.Name = string.Empty;

        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);

        var response = await DoPost(method: method, request: request, token: token, culture);

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