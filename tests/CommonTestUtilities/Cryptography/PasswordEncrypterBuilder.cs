using UserAnimeList.Domain.Security.Cryptography;
using UserAnimeList.Infrastructure.Security.Cryptography;

namespace CommonTestUtilities.Cryptography;

public class PasswordEncrypterBuilder
{
    public static IPasswordEncrypter Build() => new BCryptNet();
}