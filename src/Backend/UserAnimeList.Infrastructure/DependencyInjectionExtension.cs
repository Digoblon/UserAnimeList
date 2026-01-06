using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserAnimeList.Domain.Repositories;
using UserAnimeList.Domain.Repositories.User;
using UserAnimeList.Domain.Security.Cryptography;
using UserAnimeList.Infrastructure.Data;
using UserAnimeList.Infrastructure.Data.Repositories;
using UserAnimeList.Infrastructure.Extensions;
using UserAnimeList.Infrastructure.Security.Cryptography;

namespace UserAnimeList.Infrastructure;

public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddRepositories(services);
        AddPasswordEncrypter(services);
        AddDbContext(services, configuration);
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserAnimeListDbContext>(options =>
            options.UseSqlServer(configuration.ConnectionString()));
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