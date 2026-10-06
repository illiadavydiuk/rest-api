using Domain.Cars;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CarsConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Vin)
            .HasColumnType("varchar(17)")
            .IsRequired();

        builder.HasIndex(x => x.Vin)
            .IsUnique();

        builder.Property(x => x.Brand)
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(x => x.Model)
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnType("numeric(12,2)")
            .IsRequired();

        builder.Property(x => x.FuelType)
            .HasConversion<string>()
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(x => x.IsAvailable)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())")
            .IsRequired();
    }
}