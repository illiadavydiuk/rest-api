using Domain.Cars;
using LanguageExt;

namespace Application.Common.Interfaces.Repositories;

public interface ICarRepository
{
    Task<Option<Car>> GetById(CarId id, CancellationToken cancellationToken);
    Task<Option<Car>> GetByVin(string vin, CancellationToken cancellationToken);
    Task<Car> Add(Car car, CancellationToken cancellationToken);
    Task<Car> Update(Car car, CancellationToken cancellationToken);
    Task Delete(Car car, CancellationToken cancellationToken);
}