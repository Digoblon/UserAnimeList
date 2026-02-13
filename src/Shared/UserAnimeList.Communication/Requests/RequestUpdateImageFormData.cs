using Microsoft.AspNetCore.Http;
namespace UserAnimeList.Communication.Requests;

public class RequestUpdateImageFormData
{
    public IFormFile? Image { get; set; }
}