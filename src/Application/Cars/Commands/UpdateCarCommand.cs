using Application.Common.Interfaces;
using Domain.Cars;
using MediatR;

namespace Application.Cars.Commands;

public record UpdateCarCommand : IRequest<Car?>
{
    public required Guid CarId { get; init; }
    public required string Vin { get; init; }
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public required decimal Price { get; init; }
    public required FuelType FuelType { get; init; }
    public required bool IsAvailable { get; init; }
}

public class UpdateCarCommandHandler(ICarRepository carRepository)
    : IRequestHandler<UpdateCarCommand, Car?>
{
    public async Task<Car?> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetById(request.CarId, cancellationToken);
        if (car is null) throw new ArgumentException($"Car with ID '{request.CarId}' not found");

        var existingWithVin = await carRepository.GetByVin(request.Vin, cancellationToken);
        if (existingWithVin is not null && existingWithVin.Id != request.CarId)
        {
            throw new ArgumentException($"Car with VIN '{request.Vin}' already exists");
        }

        car.UpdateDetails(request.Vin, request.Brand, request.Model, request.Price, request.FuelType, request.IsAvailable);
        await carRepository.Update(car, cancellationToken);
        return car;
    }
}