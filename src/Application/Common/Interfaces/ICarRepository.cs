using Domain.Cars;

namespace Application.Common.Interfaces;

public interface ICarRepository
{
    Task<IReadOnlyList<Car>> GetAll(CancellationToken cancellationToken);
    Task<Car?> GetById(Guid id, CancellationToken cancellationToken);
    Task<Car?> GetByVin(string vin, CancellationToken cancellationToken);
    Task<Car> Add(Car car, CancellationToken cancellationToken);
    Task<Car> Update(Car car, CancellationToken cancellationToken);
    Task Delete(Car car, CancellationToken cancellationToken);
}