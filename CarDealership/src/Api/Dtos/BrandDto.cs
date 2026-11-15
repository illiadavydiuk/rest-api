using Domain.Brands;

namespace Api.Dtos;

public record BrandDto(
    Guid Id,
    string Name,
    string Country,
    DateTime CreatedAt)
{
    public static BrandDto FromDomainModel(Brand brand) => new(
        brand.Id.Value,
        brand.Name,
        brand.Country,
        brand.CreatedAt);
}

public record CreateBrandDto(
    string Name,
    string Country);

public record UpdateBrandDto(
    string Name,
    string Country);