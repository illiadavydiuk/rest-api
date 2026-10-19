using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Cars;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CarRepository(ApplicationDbContext context) : ICarRepository, ICarQueries
{
    public async Task<IReadOnlyList<Car>> GetAll(
        string? brand, 
        bool? isAvailable,
        decimal? minPrice, 
        decimal? maxPrice,
        CancellationToken cancellationToken)
    {
        var query = context.Cars.AsNoTracking().AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(c => c.Brand.ToLower().Contains(brand.ToLower()));
        }

        if (isAvailable.HasValue)
        {
            query = query.Where(c => c.IsAvailable == isAvailable.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(c => c.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(c => c.Price <= maxPrice.Value);
        }
        
        return await query.ToListAsync(cancellationToken); 
    }
    

    public async Task<Option<Car>> GetById(CarId id, CancellationToken cancellationToken)
    {
        var entity = await context.Cars.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity ?? Option<Car>.None;
    }

    public async Task<Option<Car>> GetByVin(string vin, CancellationToken cancellationToken)
    {
       var entity = await context.Cars.FirstOrDefaultAsync(x => 
            x.Vin == vin, cancellationToken);
        return entity ?? Option<Car>.None;
    }

    public async Task<Car> Add(Car car, CancellationToken cancellationToken)
    {
        await context.Cars.AddAsync(car, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return car;
    }

    public async Task<Car> Update(Car car, CancellationToken cancellationToken)
    {
        context.Cars.Update(car);
        await context.SaveChangesAsync(cancellationToken);
        return car;
    }

    public async Task Delete(Car car, CancellationToken cancellationToken)
    {
        context.Cars.Remove(car);
        await context.SaveChangesAsync(cancellationToken);
    }
}