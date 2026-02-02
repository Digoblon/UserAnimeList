using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserAnimeList.Domain.Entities;

public class AnimeGenreConfiguration : IEntityTypeConfiguration<AnimeGenre>
{
    public void Configure(EntityTypeBuilder<AnimeGenre> builder)
    {
        builder.ToTable("AnimeGenres");

        builder.HasKey(x => new { x.AnimeId, x.GenreId });

        builder.HasIndex(x => x.GenreId);

        builder.HasOne(x => x.Anime)
            .WithMany(a => a.Genres)
            .HasForeignKey(x => x.AnimeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Genre)
            .WithMany(g => g.Animes)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}