using CommonTestUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using UserAnimeList.Domain.Entities;
using UserAnimeList.Enums;
using UserAnimeList.Infrastructure.Data;

namespace WebApi.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    //private SqliteConnection _connection = null!;
    
    private UserAnimeList.Domain.Entities.User _user = null!;
    private UserAnimeList.Domain.Entities.Studio _studio = null!;
    private UserAnimeList.Domain.Entities.Genre _genre = null!;
    private UserAnimeList.Domain.Entities.Anime _anime = null!;
    
    private RefreshToken _refreshToken = null!;
    
    private string _password = string.Empty;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test")
            .ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UserAnimeListDbContext>));
                if (descriptor is not null)
                    services.Remove(descriptor);

                var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();


                services.AddDbContext<UserAnimeListDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseInternalServiceProvider(provider);
                });

                using var scope = services.BuildServiceProvider().CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<UserAnimeListDbContext>();

                dbContext.Database.EnsureDeleted();

                StartDatabase(dbContext);
            });
    }
    
    public string GetEmail() => _user.Email;
    public string GetPassword() => _password;
    public string GetUserName() => _user.UserName;
    public Guid GetId() => _user.Id;
    public int GetTokenVersion() => _user.TokenVersion;
    public UserRole GetRole() => _user.Role;
    public string GetRefreshToken() => _refreshToken.Token;
    
    public Guid GetStudioId() => _studio.Id;
    public string GetStudioName() => _studio.Name;

    public Guid GetGenreId() => _genre.Id;
    public string GetGenreName() => _genre.Name;

    public Guid GetAnimeId() => _anime.Id;
    public string GetAnimeName() => _anime.Name;


    private void StartDatabase(UserAnimeListDbContext dbContext)
    {
        (_user, _password) = UserBuilder.Build();
        _user.Role = UserRole.Admin;
        _studio = StudioBuilder.Build();
        _genre  = GenreBuilder.Build();
        _anime = AnimeBuilder.Build();
        AddAnimeStudioGenre();

        _refreshToken = RefreshTokenBuilder.Build(_user);

        dbContext.Users.Add(_user);
        dbContext.Studios.Add(_studio);
        dbContext.Genres.Add(_genre);
        dbContext.Animes.Add(_anime);
        dbContext.RefreshTokens.Add(_refreshToken);

        dbContext.SaveChanges();
    }

    private void AddAnimeStudioGenre()
    {
        _anime.Genres.Clear();
        var genres = new AnimeGenre()
        {
            AnimeId = _anime.Id,
            GenreId = _genre.Id
        };
        _anime.Genres.Add(genres);
        _anime.Studios.Clear();
        var studios = new AnimeStudio
        {
            StudioId = _studio.Id,
            AnimeId = _anime.Id,
        };
        _anime.Studios.Add(studios);
    }
}