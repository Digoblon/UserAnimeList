using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Infrastructure.Security.Tokens.Access.Generator;

namespace UserAnimeList.Infrastructure.Security.Tokens.Access.Validator;

public class JwtTokenValidator: JwtTokenHandler,IAccessTokenValidator
{
    private readonly string _signingKey;
    
    public JwtTokenValidator(string signingKey) => _signingKey = signingKey;

    public Guid ValidateAndGetId(string token)
    {
        var validationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = SecurityKey(_signingKey),
            ClockSkew = new TimeSpan(0)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

        var id = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        
        return Guid.Parse(id);
    }
    
}