using Application.Common.Interfaces;
using Domain.Cars;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CarRepository(ApplicationDbContext context) : ICarRepository
{
    // private readonly List<Car> _cars = [];
    // private readonly Lock _lock = new();
    
    public async Task<IReadOnlyList<Car>> GetAll(
        string? brand, 
        bool? isAvailable,
        decimal? minPrice, 
        decimal? maxPrice,
        CancellationToken cancellationToken)
    {
        var query = context.Cars.AsQueryable();
        
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
    

    public async Task<Car?> GetById(Guid id, CancellationToken cancellationToken)
    {
        // lock (_lock) return Task.FromResult<Car?>(_cars.FirstOrDefault(x => x.Id == id));
        
        return await context.Cars.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Car?> GetByVin(string vin, CancellationToken cancellationToken)
    {
        // lock (_lock) return Task.FromResult<Car?>(_cars.FirstOrDefault(x => string.Equals(x.Vin, vin, StringComparison.OrdinalIgnoreCase)));
        
        return await context.Cars.FirstOrDefaultAsync(x => 
            x.Vin == vin, cancellationToken);
    }

    public async Task<Car> Add(Car car, CancellationToken cancellationToken)
    {
        // lock (_lock) _cars.Add(car);
        // return Task.FromResult(car);
        
        await context.Cars.AddAsync(car, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return car;
    }

    public async Task<Car> Update(Car car, CancellationToken cancellationToken)
    {
        // lock (_lock)
        // {
        //     var idx = _cars.FindIndex(x => x.Id == car.Id);
        //     if (idx != -1) _cars[idx] = car;
        // }
        // return Task.FromResult(car);
        
        context.Cars.Update(car);
        await context.SaveChangesAsync(cancellationToken);
        return car;
    }

    public async Task Delete(Car car, CancellationToken cancellationToken)
    {
        // lock (_lock) _cars.RemoveAll(x => x.Id == car.Id);
        // return Task.CompletedTask;
        
        context.Cars.Remove(car);
        await context.SaveChangesAsync(cancellationToken);
    }
}