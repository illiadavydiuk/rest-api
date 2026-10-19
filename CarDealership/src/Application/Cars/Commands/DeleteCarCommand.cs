using Application.Cars.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Cars;
using MediatR;

namespace Application.Cars.Commands;

public record DeleteCarCommand : IRequest<LanguageExt.Either<CarException, Car>>
{
    public required Guid CarId { get; init; }
}

public class DeleteCarCommandHandler(ICarRepository carRepository)
    : IRequestHandler<DeleteCarCommand, LanguageExt.Either<CarException, Car>>
{
    public async Task<LanguageExt.Either<CarException, Car>> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
    {
        var carId = new CarId(request.CarId);
        var car = await carRepository.GetById(carId, cancellationToken);

        return await car.MatchAsync<Car, LanguageExt.Either<CarException, Car>>(
            async c =>
            {
                await carRepository.Delete(c, cancellationToken);
                return c;
            },
            () => new CarNotFoundException(carId));
    }
}