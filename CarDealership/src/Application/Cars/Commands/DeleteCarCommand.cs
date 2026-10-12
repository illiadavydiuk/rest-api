using Application.Common.Interfaces;
using MediatR;

namespace Application.Cars.Commands;

public record DeleteCarCommand : IRequest<bool>
{
    public required Guid CarId { get; init; }
}

public class DeleteCarCommandHandler(ICarRepository carRepository)
    : IRequestHandler<DeleteCarCommand, bool>
{
    public async Task<bool> Handle(DeleteCarCommand request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetById(request.CarId, cancellationToken);
        if (car is null) return false;

        await carRepository.Delete(car, cancellationToken);
        return true;
    }
}