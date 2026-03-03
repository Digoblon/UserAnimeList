using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Infrastructure.Data.Persistence.Configurations;

public class AnimeConfiguration : BaseEntityConfiguration<Anime>
{
    public override void Configure(EntityTypeBuilder<Anime> builder)
    {
        base.Configure(builder);

        builder.ToTable("Animes");
        
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(a => a.NameNormalized)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.HasIndex(a => a.NameNormalized)
            .IsUnique();
        
        builder.Property(a => a.Synopsis)
            .HasMaxLength(5000);

        builder.Property(a => a.Episodes);
        
        builder.Property(a => a.Status)
            .IsRequired();

        builder.Property(a => a.Source)
            .IsRequired();

        builder.Property(a => a.Type)
            .IsRequired();
        
        builder.Property(a => a.AiredFrom)
            .IsRequired(false);

        builder.Property(a => a.ImagePath)
            .IsRequired(false);
        
        builder.Property(a => a.AiredUntil)
            .IsRequired(false);
        
        builder.Ignore(a => a.Premiered);

        builder.HasMany(a => a.Genres)
            .WithOne(g => g.Anime)
            .HasForeignKey(g => g.AnimeId);
        
        builder.HasMany(a => a.Studios)
            .WithOne(s => s.Anime)
            .HasForeignKey(s => s.AnimeId);
    }
}