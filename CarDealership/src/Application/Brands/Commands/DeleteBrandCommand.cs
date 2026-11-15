using Application.Brands.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Brands;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Application.Brands.Commands;

public record DeleteBrandCommand : IRequest<Either<BrandException, Unit>>
{
    public required Guid BrandId { get; init; }
}

public class DeleteBrandCommandHandler(IBrandRepository brandRepository)
    : IRequestHandler<DeleteBrandCommand, Either<BrandException, Unit>>
{
    public async Task<Either<BrandException, Unit>> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brandId = new BrandId(request.BrandId);
        var brand = await brandRepository.GetById(brandId, cancellationToken);

        return await brand.MatchAsync(
            b => DeleteEntity(b, cancellationToken),
            () => new BrandNotFoundException(brandId));
    }

    private async Task<Either<BrandException, Unit>> DeleteEntity(Brand brand, CancellationToken cancellationToken)
    {
        try
        {
            await brandRepository.Delete(brand, cancellationToken);
            return Unit.Default;
        }
        catch (Exception ex)
        {
            return new BrandUnhandledException(brand.Id, ex);
        }
    }
}