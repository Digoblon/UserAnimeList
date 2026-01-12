using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Application.UseCases.Login.DoLogin;
using UserAnimeList.Application.UseCases.User.ChangePassword;
using UserAnimeList.Application.UseCases.User.Profile;
using UserAnimeList.Application.UseCases.User.Register;
using UserAnimeList.Application.UseCases.User.Update;

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
        var config = new TypeAdapterConfig();

        new MappingConfiguration().Register(config);

        services.AddSingleton(config);
        services.AddScoped<IAppMapper, Services.Mapper.MapsterMapper>();
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();
        services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
    }
}