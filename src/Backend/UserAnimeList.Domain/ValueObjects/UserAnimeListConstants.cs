namespace UserAnimeList.Domain.ValueObjects;

public class UserAnimeListConstants
{
    public const string DefaultCulture = "en";
    public const int RefreshTokenExpirationDays = 7;
    public const long MaxImageSize = 5 * 1024 * 1024; //5MB
    public const string ImageFilePath = "uploads";
}