using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserAnimeList.Infrastructure.Security.Tokens.Access.Generator;

public class JwtTokenHandler
{
    protected static SymmetricSecurityKey SecurityKey(string signingKey)
    {
        var bytes = Encoding.UTF8.GetBytes(signingKey);
        return new SymmetricSecurityKey(bytes);
    }
}