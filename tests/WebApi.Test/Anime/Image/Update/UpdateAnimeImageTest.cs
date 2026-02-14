using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Tokens;
using UserAnimeList.Enums;
using UserAnimeList.Exception;
using WebApi.Test.InlineData;

namespace WebApi.Test.Anime.Image.Update;

public class UpdateAnimeImageTest : UserAnimeListClassFixture
{
    private const string Method = "anime";

    private readonly Guid _id;
    private readonly int _tokenVersion;
    private readonly UserRole _userRole;
    private readonly Guid _animeId;


    public UpdateAnimeImageTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.GetId();
        _tokenVersion = factory.GetTokenVersion();
        _userRole = factory.GetRole();
        _animeId = factory.GetAnimeId();
    }

    [Fact]
    public async Task Success()
    {
        var formFile = FormFileBuilder.Png();
        var request = RequestUpdateImageFormDataBuilder.Build(formFile);
        request.Image = formFile;
        
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);
        
        var response = await DoPutMultipart($"{Method}/{_animeId}/image", request, token);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);
        
        Assert.NotNull(responseData.RootElement.GetProperty("imageUrl").GetString());
        Assert.NotEmpty(responseData.RootElement.GetProperty("imageUrl").GetString()!);
    }
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Empty_Form(string culture)
    {
        var request = RequestUpdateImageFormDataBuilder.Build();
        
        var token = JwtTokenGeneratorBuilder.Build().Generate(_id, _tokenVersion, _userRole);
        
        var response = await DoPutMultipart($"{Method}/{_animeId}/image", request,token,culture);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);
        
        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("IMAGE_NULL", new CultureInfo(culture));

        Assert.Single(errors);
        Assert.Equal(expectedMessage,errors.First().GetString());
    }
}