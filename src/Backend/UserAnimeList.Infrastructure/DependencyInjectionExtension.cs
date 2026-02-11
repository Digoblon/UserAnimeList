using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.Anime;
using UserAnimeList.Domain.Repositories.AnimeList;
using UserAnimeList.Domain.Repositories.Genre;
using UserAnimeList.Domain.Repositories.Studio;
using UserAnimeList.Domain.Repositories.Token;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Security.Cryptography;
using UserAnimeList.Domain.Security.Tokens;
using UserAnimeList.Domain.Services.DataSeed;
using UserAnimeList.Domain.Services.LoggedUser;
using UserAnimeList.Infrastructure.Data;
using UserAnimeList.Infrastructure.Data.Repositories;
using UserAnimeList.Infrastructure.Data.Seed;
using UserAnimeList.Infrastructure.Extensions;
using UserAnimeList.Infrastructure.Security.Cryptography;
using UserAnimeList.Infrastructure.Security.Tokens.Access.Generator;
using UserAnimeList.Infrastructure.Security.Tokens.Access.Validator;
using UserAnimeList.Infrastructure.Security.Tokens.Refresh;
using UserAnimeList.Infrastructure.Services.LoggedUser;

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
        //if(configuration.IsUnitTestEnvironment()) return;
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
        services.AddScoped<ILoggedUser, LoggedUser>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<IRefreshTokenValidator, RefreshTokenValidator>();
        services.AddScoped<IStudioRepository, StudioRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IAnimeRepository, AnimeRepository>();
        services.AddScoped<IAnimeListRepository, AnimeListRepository>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

    } 

    private static void AddPasswordEncrypter(IServiceCollection services)
    {
        services.AddScoped<IPasswordEncrypter, BCryptNet>();
    }
}