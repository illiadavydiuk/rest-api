using Domain.Brands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    ApplicationDbContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
            await SeedAsync();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An error occurred during initialisation.");
            throw;
        }
    }

    private async Task SeedAsync()
    {
        if (await context.Brands.AnyAsync())
        {
            return;
        }

        var brands = new List<Brand>
        {
            Brand.New(BrandId.New(), "Toyota", "Japan"),
            Brand.New(BrandId.New(), "Nissan", "Japan"),
            Brand.New(BrandId.New(), "BMW", "Germany"),
            Brand.New(BrandId.New(), "Kia", "South Korea"),
            Brand.New(BrandId.New(), "Tesla", "USA")
        };

        await context.Brands.AddRangeAsync(brands);
        await context.SaveChangesAsync();
    }
}