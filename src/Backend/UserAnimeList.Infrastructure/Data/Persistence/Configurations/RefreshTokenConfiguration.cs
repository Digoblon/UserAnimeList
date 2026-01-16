using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserAnimeList.Domain.Entities;

namespace UserAnimeList.Infrastructure.Data.Persistence.Configurations;

public class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("RefreshTokens");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(512);
        
        builder.HasIndex(t => t.Token)
            .IsUnique();
        
        builder.Property(t => t.RevokedOn)
            .IsRequired(false);
        
        builder.HasOne(t => t.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}