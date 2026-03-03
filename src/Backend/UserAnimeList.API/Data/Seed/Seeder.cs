using UserAnimeList.Domain.Services.DataSeed;

namespace UserAnimeList.Data.Seed;

public static class Seeder
{
    public static async Task SeedAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        await seeder.SeedAsync();
    }
}