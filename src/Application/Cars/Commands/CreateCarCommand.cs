using Application.Common.Interfaces;
using Domain.Cars;
using MediatR;

namespace Application.Cars.Commands;

public record CreateCarCommand : IRequest<Car>
{
    public required string Vin { get; init; }
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public required decimal Price { get; init; }
    public required FuelType FuelType { get; init; }
    public required bool IsAvailable { get; init; }
}

public class CreateCarCommandHandeler(ICarRepository carRepository) : IRequestHandler<CreateCarCommand, Car>
{
    public async Task<Car> Handle(CreateCarCommand request, CancellationToken cancellationToken)
    {
        var existingCar = await carRepository.GetByVin(request.Vin, cancellationToken);

        if (existingCar != null)
        {
            throw new ArgumentException($"Car with Vin '{request.Vin}' already exists");
        }

        var car = Car.New(
            Guid.NewGuid(),
            request.Vin,
            request.Brand,
            request.Model,
            request.Price,
            request.FuelType,
            request.IsAvailable);

        await carRepository.Add(car, cancellationToken);
        return car;
    }
}
