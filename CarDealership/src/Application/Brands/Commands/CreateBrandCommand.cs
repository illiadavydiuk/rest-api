using Application.Brands.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Brands;
using LanguageExt;
using MediatR;

namespace Application.Brands.Commands;

public record CreateBrandCommand : IRequest<Either<BrandException, Brand>>
{
    public required string Name { get; init; }
    public required string Country { get; init; }
}

public class CreateBrandCommandHandler(IBrandRepository brandRepository)
    : IRequestHandler<CreateBrandCommand, Either<BrandException, Brand>>
{
    public async Task<Either<BrandException, Brand>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var existingBrand = await brandRepository.GetByName(request.Name, cancellationToken);

        return await existingBrand.MatchAsync(
            b => new BrandAlreadyExistsException(b.Id, b.Name),
            () => CreateEntity(request, cancellationToken));
    }

    private async Task<Either<BrandException, Brand>> CreateEntity(
        CreateBrandCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var brand = Brand.New(
                BrandId.New(),
                request.Name,
                request.Country);

            await brandRepository.Add(brand, cancellationToken);
            return brand;
        }
        catch (Exception ex)
        {
            return new BrandUnhandledException(BrandId.Empty(), ex);
        }
    }
}