using System.ComponentModel.DataAnnotations;
using Domain.Cars;

namespace Api.Dtos;

public record CarDto(
    Guid Id,
    string Vin,
    string Brand,
    string Model,
    decimal Price,
    FuelType FuelType,
    bool IsAvailable,
    DateTime CreatedAt)
{
    public static CarDto FromDomainModel(Car car)
        => new(car.Id, car.Vin, car.Brand, car.Model, car.Price, car.FuelType, car.IsAvailable, car.CreatedAt);
}

public record CreateCarDto(string Vin, string Brand, string Model, decimal Price, FuelType FuelType, bool IsAvailable);
public record UpdateCarDto(string Vin, string Brand, string Model, decimal Price, FuelType FuelType, bool IsAvailable);