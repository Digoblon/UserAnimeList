using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Infrastructure.Data.Persistence.Configurations;

public class GenreConfiguration : BaseEntityConfiguration<Genre>
{
    public override void Configure(EntityTypeBuilder<Genre> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Genres");
        
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(s => s.NameNormalized)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(u => u.NameNormalized).IsUnique().HasFilter("[DeletedOn] IS NULL");
        
        builder.Property(s => s.Description)
            .HasMaxLength(2000);
        
        builder.HasQueryFilter(s => s.IsActive && s.DeletedOn == null);
        
        builder.HasMany(g => g.Animes)
            .WithOne(a => a.Genre)
            .HasForeignKey(a => a.GenreId);
    }
}