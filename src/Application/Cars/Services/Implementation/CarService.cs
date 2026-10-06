using Application.Cars.Services.Abstract;
using Application.Common.Interfaces;
using Domain.Cars;

namespace Application.Cars.Services.Implementation;

public class CarService(ICarRepository carRepository) : ICarService
{
    public async Task<IReadOnlyList<Car>> GetCars(CancellationToken cancellationToken)
    {
        return await carRepository.GetAll(cancellationToken);
    }

    public async Task<Car?> GetCar(Guid id, CancellationToken cancellationToken)
    {
        return await carRepository.GetById(id, cancellationToken);
    }

    public async Task<Car> Add(string vin, string brand, string model, decimal price, FuelType fuelType, bool isAvailable,
        CancellationToken cancellationToken)
    {
        var existing = await carRepository.GetByVin(vin, cancellationToken);
        if (existing != null)
            throw new ArgumentException($"Car with this VIN '{vin}' already exists");
        
        var car = Car.New(Guid.NewGuid(), vin, brand, model, price, fuelType, isAvailable);
        return await carRepository.Add(car, cancellationToken);
    }

    public async Task<Car?> Update(Guid id, string vin, string brand, string model, decimal price, FuelType fuelType, bool isAvailable,
        CancellationToken cancellationToken)
    {
        var car = await carRepository.GetById(id, cancellationToken);
        if (car == null) return null;
        
        var existingWithVin = await carRepository.GetByVin(vin, cancellationToken);
        if (existingWithVin != null && existingWithVin.Id != id)
            throw new ArgumentException($"Car with this VIN '{vin}' already exists");
        
        car.UpdateDetails(vin, brand, model, price, fuelType, isAvailable);
        return await carRepository.Update(car, cancellationToken);
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetById(id, cancellationToken);
        if (car == null) return false;
        
        await carRepository.Delete(car, cancellationToken);
        return true;
    }
}