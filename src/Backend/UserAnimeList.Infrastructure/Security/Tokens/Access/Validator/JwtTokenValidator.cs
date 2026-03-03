using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Enums;
using UserAnimeList.Exception;
using UserAnimeList.Infrastructure.Security.Tokens.Access.Generator;

namespace UserAnimeList.Infrastructure.Security.Tokens.Access.Validator;

public class JwtTokenValidator: JwtTokenHandler,IAccessTokenValidator
{
    private readonly string _signingKey;
    
    public JwtTokenValidator(string signingKey) => _signingKey = signingKey;

    public AccessTokenData Validate(string token)
    {
        var validationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = SecurityKey(_signingKey),
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

        var userId = Guid.Parse(
            principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
        );

        var tokenVersion = int.Parse(
            principal.Claims.First(c => c.Type == "token_version").Value
        );
        
        var roleClaim = principal.Claims.First(c => c.Type == ClaimTypes.Role).Value;

        if (!Enum.TryParse<UserRole>(roleClaim, out var role))
            throw new SecurityTokenException(ResourceMessagesException.INVALID_ROLE_CLAIM);
        
        
        return new AccessTokenData(userId, tokenVersion, role);
    }
    
}