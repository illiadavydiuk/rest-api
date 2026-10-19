using Domain.Cars;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface ICarQueries
{
    Task<IReadOnlyList<Car>> GetAll(
        string? brand, 
        bool? isAvailable,
        decimal? minPrice, 
        decimal? maxPrice,
        CancellationToken cancellationToken);
    Task<Option<Car>> GetById(CarId id, CancellationToken cancellationToken);
}