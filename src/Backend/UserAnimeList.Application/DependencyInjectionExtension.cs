using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Application.UseCases.User.Register;

namespace UserAnimeList.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddUseCases(services);
        AddMapster(services);
    }

    private static void AddMapster(IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(typeof(DependencyInjectionExtension).Assembly);

        services.AddSingleton(config);
        
        services.AddScoped<IAppMapper, Services.Mapper.MapsterMapper>();
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
    }
}