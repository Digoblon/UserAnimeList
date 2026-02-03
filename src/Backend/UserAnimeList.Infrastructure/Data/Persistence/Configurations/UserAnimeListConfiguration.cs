using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserAnimeList.Infrastructure.Data.Persistence.Configurations;

public class UserAnimeListConfiguration : BaseEntityConfiguration<Domain.Entities.UserAnimeList>
{
    public override void Configure(EntityTypeBuilder<Domain.Entities.UserAnimeList> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("UserAnimeLists");
        
        builder.HasIndex(x => new  { x.UserId, x.AnimeId })
            .IsUnique();
        
        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Score)
            .IsRequired(false);
        
        builder.Property(x => x.Progress)
            .IsRequired(false);
        
        builder.Property(x => x.DateStarted)
            .IsRequired(false);
        
        builder.Property(x => x.DateFinished)
            .IsRequired(false);
        
        builder.HasOne(x => x.User)
            .WithMany(u => u.AnimeList) 
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Anime)
            .WithMany(a => a.UserEntries) 
            .HasForeignKey(x => x.AnimeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.AnimeId);
        builder.HasIndex(x => x.Status);
    }
}