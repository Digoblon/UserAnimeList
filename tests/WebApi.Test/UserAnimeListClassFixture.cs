using System.Globalization;
using System.Reflection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UserAnimeList.Communication.Requests;

namespace WebApi.Test;

public class UserAnimeListClassFixture: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public UserAnimeListClassFixture(CustomWebApplicationFactory  factory) => _httpClient = factory.CreateClient();
    
    protected async Task<HttpResponseMessage> DoPost(string method, object request,string token = "", string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);
        
        return await _httpClient.PostAsJsonAsync(method,request);
    }

    protected async Task<HttpResponseMessage> DoGet(string method, string token = "", string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);
        
        return await _httpClient.GetAsync(method);
    }
    protected async Task<HttpResponseMessage> DoGetQuery(string method,object request, string token = "", string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);

        var query = BuildQueryString(request);
        var url = string.IsNullOrWhiteSpace(query) ? method : $"{method}?{query}";

        return await _httpClient.GetAsync(url);
    }
    
    protected async Task<HttpResponseMessage> DoDelete(string method, string token = "", string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);
        
        return await _httpClient.DeleteAsync(method);
    }
 
    protected async Task<HttpResponseMessage> DoPut(string method,object request, string token = "", string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);
        
        return await _httpClient.PutAsJsonAsync(method, request);
    }
    
    protected async Task<HttpResponseMessage> DoPutMultipart(string method, RequestUpdateImageFormData request, string token = "", string culture = "en")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(token);

        var content = new MultipartFormDataContent();

        var file = request.Image;

        if (file is not null)
        {
            byte[] bytes;
            await using (var memoryStream = new MemoryStream())
            {
                await request.Image!.CopyToAsync(memoryStream);
                bytes = memoryStream.ToArray();
            }

            var fileContent = new ByteArrayContent(bytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(request.Image.ContentType);

            content.Add(fileContent, "Image", request.Image.FileName);
        }
        else
        {
            content.Add(new StringContent(string.Empty), "Image");
        }
        
        return await _httpClient.PutAsync(method, content);
    }
    
    
    
    

    private static string BuildQueryString(object request)
    {
        var properties = request
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanRead)
            .Select(p => (Name: p.Name, Value: p.GetValue(request)))
            .Where(p => p.Value is not null)
            .Select(p => $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(FormatQueryValue(p.Value!))}");

        return string.Join("&", properties);
    }

    private static string FormatQueryValue(object value)
    {
        return value switch
        {
            DateOnly dateOnly => dateOnly.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty
        };
    }

    private void ChangeRequestCulture(string culture)
    {
        if(_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
            _httpClient.DefaultRequestHeaders.Remove("Accept-Language");
        
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);
    }

    private void AuthorizeRequest(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}