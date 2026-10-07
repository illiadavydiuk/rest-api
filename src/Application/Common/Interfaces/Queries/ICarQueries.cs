using Domain.Cars;

namespace Application.Common.Interfaces.Queries;

public interface ICarQueries
{
    Task<IReadOnlyList<Car>> GetAll(
        string? brand, 
        bool? isAvailable,
        decimal? minPrice, 
        decimal? maxPrice,
        CancellationToken cancellationToken);
    Task<Car?> GetById(Guid id, CancellationToken cancellationToken);
}