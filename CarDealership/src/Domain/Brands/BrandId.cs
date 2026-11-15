namespace Domain.Brands;

public record BrandId(Guid Value)
{
    public static BrandId New() => new(Guid.NewGuid());
}