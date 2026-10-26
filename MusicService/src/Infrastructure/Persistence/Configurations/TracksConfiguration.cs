using Domain.Tracks;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TracksConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new TrackId(x));

        builder.Property(x => x.Isrc)
            .HasColumnType("varchar(12)")
            .IsRequired();

        builder.HasIndex(x => x.Isrc)
            .IsUnique();

        builder.Property(x => x.Title)
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(x => x.Artist)
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(x => x.DurationSeconds)
            .IsRequired();

        builder.Property(x => x.Genre)
            .HasConversion<string>()
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(x => x.IsExplicit)
            .IsRequired();

        builder.Property(x => x.ReleaseDate)
            .HasColumnType("date")
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())")
            .IsRequired();
    }
}