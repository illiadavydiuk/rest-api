using Application.Brands.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Brands;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Application.Brands.Commands;

public record UpdateBrandCommand : IRequest<Either<BrandException, Brand>>
{
    public required Guid BrandId { get; init; }
    public required string Name { get; init; }
    public required string Country { get; init; }
}

public class UpdateBrandCommandHandler(IBrandRepository brandRepository)
    : IRequestHandler<UpdateBrandCommand, Either<BrandException, Brand>>
{
    public async Task<Either<BrandException, Brand>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var brandId = new BrandId(request.BrandId);
        var brand = await brandRepository.GetById(brandId, cancellationToken);

        return await brand.MatchAsync(
            b => CheckDuplicates(b.Id, request.Name, cancellationToken)
                .BindAsync(_ => UpdateEntity(b, request, cancellationToken)),
            () => new BrandNotFoundException(brandId));
    }

    private async Task<Either<BrandException, Unit>> CheckDuplicates(
        BrandId currentBrandId,
        string name,
        CancellationToken cancellationToken)
    {
        var brand = await brandRepository.GetByName(name, cancellationToken);
        return brand.Match<Either<BrandException, Unit>>(
            b => b.Id == currentBrandId ? Unit.Default : new BrandAlreadyExistsException(b.Id, b.Name),
            () => Unit.Default);
    }

    private async Task<Either<BrandException, Brand>> UpdateEntity(
        Brand brand,
        UpdateBrandCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            brand.UpdateDetails(request.Name, request.Country);
            await brandRepository.Update(brand, cancellationToken);
            return brand;
        }
        catch (Exception ex)
        {
            return new BrandUnhandledException(brand.Id, ex);
        }
    }
}