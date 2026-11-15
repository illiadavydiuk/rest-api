using Application.Brands.Exceptions;
using Application.Cars.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Brands;
using Domain.Cars;
using LanguageExt;
using MediatR;

namespace Application.Cars.Commands;

public record CreateCarCommand : IRequest<Either<CarException, Car>>
{
    public required string Vin { get; init; }
    public required Guid BrandId { get; init; }
    public required string Model { get; init; }
    public required decimal Price { get; init; }
    public required FuelType FuelType { get; init; }
    public required bool IsAvailable { get; init; }
}

public class CreateCarCommandHandler(
    ICarRepository carRepository,
    IBrandRepository brandRepository) 
    : IRequestHandler<CreateCarCommand, Either<CarException, Car>>
{
    public async Task<Either<CarException, Car>> Handle(CreateCarCommand request, CancellationToken cancellationToken)
    {
        var brandId = new BrandId(request.BrandId);
        var existingBrand = await brandRepository.GetById(brandId, cancellationToken);

        return await existingBrand.MatchAsync(
            async brand =>
            {
                var existingCar = await carRepository.GetByVin(request.Vin, cancellationToken);

                return await existingCar.MatchAsync(
                    c => new CarAlreadyExistsException(c.Id),
                    () => CreateEntity(request, brand.Id, cancellationToken));
            },
            () => Task.FromResult<Either<CarException, Car>>(new BrandNotFoundForCarException(brandId)));
    }

    private async Task<Either<CarException, Car>> CreateEntity(
        CreateCarCommand request,
        BrandId brandId,
        CancellationToken cancellationToken)
    {
        try
        {
            var car = Car.New(
                CarId.New(),
                request.Vin,
                brandId,
                request.Model,
                request.Price,
                request.FuelType,
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