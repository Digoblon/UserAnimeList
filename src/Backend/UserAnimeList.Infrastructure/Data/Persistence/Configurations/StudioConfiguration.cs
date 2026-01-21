using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Infrastructure.Data.Persistence.Configurations;

public class StudioConfiguration : BaseEntityConfiguration<Studio>
{
    public override void Configure(EntityTypeBuilder<Studio> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Studios");
        
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(s => s.NameNormalized)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(u => u.NameNormalized).IsUnique();
        
        builder.Property(s => s.Description)
            .HasMaxLength(2000);
        
    }
}