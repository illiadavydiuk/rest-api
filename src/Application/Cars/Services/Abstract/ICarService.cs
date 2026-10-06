using Domain.Cars;

namespace Application.Cars.Services.Abstract;

public interface ICarService
{
    Task<IReadOnlyList<Car>> GetCars(CancellationToken cancellationToken);
    Task<Car?> GetCar(Guid id, CancellationToken cancellationToken);
    Task<Car> Add(string vin, string brand, string model, decimal price, FuelType fuelType, bool isAvailable, CancellationToken cancellationToken);
    Task<Car?> Update(Guid id, string vin, string brand, string model, decimal price, FuelType fuelType, bool isAvailable, CancellationToken cancellationToken);
    Task<bool> Delete(Guid id, CancellationToken cancellationToken);
}