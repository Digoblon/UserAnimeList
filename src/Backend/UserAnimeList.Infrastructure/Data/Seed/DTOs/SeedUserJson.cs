namespace UserAnimeList.Infrastructure.Data.Seed.DTOs;

//CLASSE GERADA POR INTELIGÊNCIA ARTIFICIAL
public sealed class SeedUserJson
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Ex.: "User" | "Admin" (case-insensitive)
    public string Role { get; set; } = "User";
}