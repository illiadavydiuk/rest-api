using Domain.Cars;

namespace Api.Dtos;

public record CarDto(
    Guid Id,
    string Vin,
    Guid BrandId,
    string BrandName,
    string Model,
    decimal Price,
    FuelType FuelType,
    bool IsAvailable,
    DateTime CreatedAt)
{
    public static CarDto FromDomainModel(Car car)
        => new(
            car.Id.Value,
            car.Vin,
            car.BrandId.Value,
            car.Brand?.Name ?? string.Empty,
            car.Model,
            car.Price,
            car.FuelType,
            car.IsAvailable,
            car.CreatedAt);
}

public record CreateCarDto(
    string Vin,
    Guid BrandId,
    string Model,
    decimal Price,
    FuelType FuelType,
    bool IsAvailable);

public record UpdateCarDto(
    string Vin,
    Guid BrandId,
    string Model,
    decimal Price,
    FuelType FuelType,
    bool IsAvailable);