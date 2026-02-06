using Microsoft.EntityFrameworkCore;
using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Infrastructure.Data;

public class UserAnimeListDbContext(DbContextOptions<UserAnimeListDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Studio>  Studios { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Anime> Animes { get; set; }
    public DbSet<AnimeStudio> AnimeStudios { get; set; }
    public DbSet<AnimeGenre> AnimeGenres { get; set; }
    public DbSet<AnimeList>  AnimeLists { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserAnimeListDbContext).Assembly);
    }
}

