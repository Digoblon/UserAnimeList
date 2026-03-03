using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using UserAnimeList.Application.Services.Mapper;
using UserAnimeList.Application.UseCases.Anime.Delete.SoftDelete;
using UserAnimeList.Application.UseCases.Anime.Filter;
using UserAnimeList.Application.UseCases.Anime.Get.ById;
using UserAnimeList.Application.UseCases.Anime.Image;
using UserAnimeList.Application.UseCases.Anime.Image.Delete;
using UserAnimeList.Application.UseCases.Anime.Image.Update;
using UserAnimeList.Application.UseCases.Anime.Register;
using UserAnimeList.Application.UseCases.Anime.Search;
using UserAnimeList.Application.UseCases.Anime.Update;
using UserAnimeList.Application.UseCases.AnimeList.AddEntry;
using UserAnimeList.Application.UseCases.AnimeList.Delete;
using UserAnimeList.Application.UseCases.AnimeList.Get.ById;
using UserAnimeList.Application.UseCases.AnimeList.List.ByUserId;
using UserAnimeList.Application.UseCases.AnimeList.List.Me;
using UserAnimeList.Application.UseCases.AnimeList.Update;
using UserAnimeList.Application.UseCases.Genre.Delete.SoftDelete;
using UserAnimeList.Application.UseCases.Genre.Get.ById;
using UserAnimeList.Application.UseCases.Genre.Get.ByName;
using UserAnimeList.Application.UseCases.Genre.Register;
using UserAnimeList.Application.UseCases.Genre.Update;
using UserAnimeList.Application.UseCases.Login.DoLogin;
using UserAnimeList.Application.UseCases.Studio.Delete.SoftDelete;
using UserAnimeList.Application.UseCases.Studio.Get.ById;
using UserAnimeList.Application.UseCases.Studio.Get.ByName;
using UserAnimeList.Application.UseCases.Studio.Register;
using UserAnimeList.Application.UseCases.Studio.Update;
using UserAnimeList.Application.UseCases.Token.RefreshToken;
using UserAnimeList.Application.UseCases.User.ChangePassword;
using UserAnimeList.Application.UseCases.User.Delete.SoftDelete;
using UserAnimeList.Application.UseCases.User.Image;
using UserAnimeList.Application.UseCases.User.Image.Delete;
using UserAnimeList.Application.UseCases.User.Image.Update;
using UserAnimeList.Application.UseCases.User.Profile;
using UserAnimeList.Application.UseCases.User.Register;
using UserAnimeList.Application.UseCases.User.Update;

namespace UserAnimeList.Application;

public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services)
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
        //user
        services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
        services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
        services.AddScoped<ISoftDeleteUserUseCase, SoftDeleteUserUseCase>();
        services.AddScoped<IUpdateUserImageUseCase, UpdateUserImageUseCase>();
        services.AddScoped<IDeleteUserImageUseCase, DeleteUserImageUseCase>();
        //login
        services.AddScoped<IDoLoginUseCase, DoLoginUseCase>();
        //RefreshToken
        services.AddScoped<IUseRefreshTokenUseCase, UseRefreshTokenUseCase>();
        //Studio
        services.AddScoped<IRegisterStudioUseCase, RegisterStudioUseCase>();
        services.AddScoped<IGetStudioByIdUseCase, GetStudioByIdUseCase>();
        services.AddScoped<IGetStudioByNameUseCase, GetStudioByNameUseCase>();
        services.AddScoped<IUpdateStudioUseCase, UpdateStudioUseCase>();
        services.AddScoped<ISoftDeleteStudioUseCase, SoftDeleteStudioUseCase>();
        //Genre
        services.AddScoped<IRegisterGenreUseCase, RegisterGenreUseCase>();
        services.AddScoped<IGetGenreByIdUseCase, GetGenreByIdUseCase>();
        services.AddScoped<IGetGenreByNameUseCase, GetGenreByNameUseCase>();
        services.AddScoped<IUpdateGenreUseCase, UpdateGenreUseCase>();
        services.AddScoped<ISoftDeleteGenreUseCase, SoftDeleteGenreUseCase>();
        //Anime
        services.AddScoped<IRegisterAnimeUseCase, RegisterAnimeUseCase>();
        services.AddScoped<IUpdateAnimeUseCase, UpdateAnimeUseCase>();
        services.AddScoped<IGetAnimeByIdUseCase, GetAnimeByIdUseCase>();
        services.AddScoped<ISoftDeleteAnimeUseCase, SoftDeleteAnimeUseCase>();
        services.AddScoped<ISearchAnimeUseCase, SearchAnimeUseCase>();
        services.AddScoped<IFilterAnimeUseCase, FilterAnimeUseCase>();
        services.AddScoped<IUpdateAnimeImageUseCase, UpdateAnimeImageUseCase>();
        services.AddScoped<IDeleteAnimeImageUseCase, DeleteAnimeImageUseCase>();
        //AnimeList
        services.AddScoped<IAddAnimeListEntryUseCase, AddAnimeListEntryUseCase>();
        services.AddScoped<IUpdateAnimeListEntryUseCase, UpdateAnimeListEntryUseCase>();
        services.AddScoped<IDeleteAnimeListEntryUseCase, DeleteAnimeListEntryUseCase>();
        services.AddScoped<IGetAnimeListEntryByIdUseCase, GetAnimeListEntryByIdUseCase>();
        services.AddScoped<IListAnimeByUserIdUseCase, ListAnimeByUserIdUseCase>();
        services.AddScoped<IListAnimeUseCase, ListAnimeUseCase>();
    }
}