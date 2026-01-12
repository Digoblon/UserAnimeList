using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.Register;

public class RegisterUserTest : UserAnimeListClassFixture
{
    private readonly string method = "user";
    public RegisterUserTest(CustomWebApplicationFactory factory) : base(factory) { }
    
    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        var response = await DoPost(method:method, request:request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);
        
        Assert.NotNull(responseData.RootElement.GetProperty("userName").GetString());
        Assert.NotEmpty(responseData.RootElement.GetProperty("userName").GetString()!);
        Assert.Equal(request.UserName, responseData.RootElement.GetProperty("userName").GetString());
        
        //responseData.RootElement.GetProperty("name").GetString().Should().NotBeNullOrWhiteSpace().And.Be(request.Name);
        //responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().Should().NotBeNullOrEmpty();
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Empty_Name(string culture)
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.UserName = string.Empty;
        
        var response = await DoPost(method:method, request:request, culture:culture);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        //response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        
        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));
        
        Assert.Single(errors);
        Assert.Equal(expectedMessage,errors.First().ToString());
        //errors.Should().ContainSingle().And.Contain(error => error.GetString()!.Equals(expectedMessage));
    }
}