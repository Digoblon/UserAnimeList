using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Infrastructure.Data.Persistence.Configurations;

public class AnimeStudioConfiguration : IEntityTypeConfiguration<AnimeStudio>
{
    public void Configure(EntityTypeBuilder<AnimeStudio> builder)
    {
        builder.ToTable("AnimeStudios");
        
        builder.HasKey(x => new { x.AnimeId, x.StudioId });

        builder.HasIndex(x => x.StudioId);

        builder.HasOne(x => x.Anime)
            .WithMany(a => a.Studios)
            .HasForeignKey(x => x.AnimeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Studio)
            .WithMany(s => s.Animes)
            .HasForeignKey(x => x.StudioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}