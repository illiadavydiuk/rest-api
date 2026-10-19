using Application.Cars.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Cars;
using LanguageExt;
using MediatR;

namespace Application.Cars.Commands;

public record CreateCarCommand : IRequest<Either<CarException, Car>>
{
    public required string Vin { get; init; }
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public required decimal Price { get; init; }
    public required FuelType FuelType { get; init; }
    public required bool IsAvailable { get; init; }
}

public class CreateCarCommandHandler(ICarRepository carRepository) 
    : IRequestHandler<CreateCarCommand, Either<CarException, Car>>
{
    public async Task<Either<CarException, Car>> Handle(CreateCarCommand request, CancellationToken cancellationToken)
    {
        var existingCar = await carRepository.GetByVin(request.Vin, cancellationToken);
        
        return await existingCar.MatchAsync(
            c => new CarAlreadyExistsException(c.Id),
            () => CreateEntity(request, cancellationToken));
    }

    private async Task<Either<CarException, Car>> CreateEntity(CreateCarCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var car = Car.New(CarId.New(), request.Vin, request.Brand, request.Model, request.Price, request.FuelType, 
                request.IsAvailable);
            await carRepository.Add(car, cancellationToken);
            return car;
        }
        catch (Exception ex)
        {
            return new CarUnhandledException(CarId.Empty(), ex);
        }
    }
}
