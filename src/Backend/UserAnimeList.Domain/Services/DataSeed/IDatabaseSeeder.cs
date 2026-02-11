namespace UserAnimeList.Domain.Services.DataSeed;

public interface IDatabaseSeeder
{
    Task SeedAsync();
}