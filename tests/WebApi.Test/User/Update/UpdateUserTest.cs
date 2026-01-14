using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.User.Update;

public class UpdateUserTest : UserAnimeListClassFixture
{
private const string METHOD = "user";

private readonly Guid _id;
private readonly int _tokenVersion;


public UpdateUserTest(CustomWebApplicationFactory factory) : base(factory)
{
    _id = factory.GetId();
    _tokenVersion = factory.GetTokenVersion();
}

[Fact]
public async Task Success()
{
    var request = RequestUpdateUserJsonBuilder.Build();
        
    var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion);
        
    var response = await DoPut(METHOD, request, token);

    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
}
    
[Theory]
[ClassData(typeof(CultureInlineDataTest))]
public async Task Error_Empty_UserName(string culture)
{
    var request = RequestUpdateUserJsonBuilder.Build();
    request.UserName = string.Empty;
        
    var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion);
        
    var response = await DoPut(METHOD, request,token,culture);
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
    await using var responseBody = await response.Content.ReadAsStreamAsync();
    var responseData = await JsonDocument.ParseAsync(responseBody);
        
    var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

    var expectedMessage = ResourceMessagesException.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

    Assert.Single(errors);
    Assert.Equal(expectedMessage,errors.First().GetString());
}

    
}