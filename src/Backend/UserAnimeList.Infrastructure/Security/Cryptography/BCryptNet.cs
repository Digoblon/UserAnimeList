using UserAnimeList.Domain.Security.Cryptography;

namespace UserAnimeList.Infrastructure.Security.Cryptography;

public class BCryptNet : IPasswordEncrypter
{
    public string Encrypt(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool IsValid(string password, string passwordHash) => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}