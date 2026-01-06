using System.ComponentModel;
using System.Text.Json.Serialization;
using UserAnimeList.Communication.JsonConverters;

namespace UserAnimeList.Communication.Requests;

public class RequestRegisterUserJson
{
    [JsonConverter(typeof(SanitizedStringConverter))]
    public string UserName { get; set; } = string.Empty;
    
    [JsonConverter(typeof(SanitizedStringConverter))]
    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
}