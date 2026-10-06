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

public record CreateCarDto(
    [Required, StringLength(17, MinimumLength = 17)] string Vin,
    [Required, MaxLength(100)] string Brand,
    [Required, MaxLength(100)] string Model,
    [Range(0.01, 10_000_000)] decimal Price,
    [Required] FuelType FuelType,
    bool IsAvailable);

public record UpdateCarDto(
    [Required, StringLength(17, MinimumLength = 17)] string Vin,
    [Required, MaxLength(100)] string Brand,
    [Required, MaxLength(100)] string Model,
    [Range(0.01, 10_000_000)] decimal Price,
    [Required] FuelType FuelType,
    bool IsAvailable);