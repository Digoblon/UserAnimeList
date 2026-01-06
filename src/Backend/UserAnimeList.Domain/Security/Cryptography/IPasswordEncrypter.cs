namespace UserAnimeList.Domain.Security.Cryptography;

public interface IPasswordEncrypter
{
    public string Encrypt(string password);
    public bool IsValid(string password, string passwordHash);
}