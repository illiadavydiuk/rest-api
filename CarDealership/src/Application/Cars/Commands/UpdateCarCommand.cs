using Application.Cars.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Cars;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Application.Cars.Commands;

public record UpdateCarCommand : IRequest<Either<CarException, Car>>
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
    : IRequestHandler<UpdateCarCommand, Either<CarException, Car>>
{
    public async Task<Either<CarException, Car>> Handle(UpdateCarCommand request, CancellationToken cancellationToken)
    {
        var carId = new CarId(request.CarId);
        var car = await carRepository.GetById(carId, cancellationToken);

        return await car.MatchAsync(
            c => CheckDuplicates(c.Id, request.Vin, cancellationToken)
                .BindAsync(_ => UpdateEntity(c, request, cancellationToken)),
            () => new CarNotFoundException(carId));
    }

    private async Task<Either<CarException, Unit>> CheckDuplicates(CarId currentCarId, string vin, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetByVin(vin, cancellationToken);
        return car.Match<Either<CarException, Unit>>(
            c => c.Id == currentCarId ? Unit.Default : new CarAlreadyExistsException(c.Id),
            () => Unit.Default);
    }

    private async Task<Either<CarException, Car>> UpdateEntity(Car car, UpdateCarCommand request, CancellationToken cancellationToken)
    {
        try
        {
            car.UpdateDetails(request.Vin, request.Brand, request.Model, request.Price, request.FuelType, request.IsAvailable);
            await carRepository.Update(car, cancellationToken);
            return car;
        }
        catch (Exception ex)
        {
            return new CarUnhandledException(car.Id, ex);
        }
    }
}