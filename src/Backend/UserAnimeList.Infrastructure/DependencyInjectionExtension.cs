using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Security.Cryptography;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Infrastructure.Data;
using UserAnimeList.Infrastructure.Data.Repositories;
using UserAnimeList.Infrastructure.Extensions;
using UserAnimeList.Infrastructure.Security.Cryptography;
using UserAnimeList.Infrastructure.Security.Tokens.Access.Generator;
using UserAnimeList.Infrastructure.Security.Tokens.Access.Validator;

namespace UserAnimeList.Infrastructure;

public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddRepositories(services);
        AddPasswordEncrypter(services);
        AddTokens(services, configuration);
        AddDbContext(services, configuration);
    }

    

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserAnimeListDbContext>(options =>
            options.UseSqlServer(configuration.ConnectionString()));
    }
    
    private static void AddTokens(IServiceCollection services, IConfiguration configuration)
    {
        var expirationTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpirationTimeMinutes");
        var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");
        
        services.AddScoped<IAccessTokenGenerator>(options => new JwtTokenGenerator(expirationTimeMinutes, signingKey!));
        services.AddScoped<IAccessTokenValidator>(options => new JwtTokenValidator(signingKey!));
        
        //services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();

    } 

    private static void AddPasswordEncrypter(IServiceCollection services)
    {
        services.AddScoped<IPasswordEncrypter, BCryptNet>();
    }
}