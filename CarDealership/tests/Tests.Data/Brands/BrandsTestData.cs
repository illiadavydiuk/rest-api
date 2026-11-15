using Domain.Brands;

namespace Tests.Data.Brands;

public static class BrandsTestData
{
    public static Brand CreateTestBrandForCars() => Brand.New(
        BrandId.New(),
        $"TestBrand-{Guid.NewGuid():N}",
        "Japan");

    public static Brand CreateValidTestBrand() => Brand.New(
        BrandId.New(),
        "Subaru",
        "Japan");

    public static Brand CreateTakenTestBrand() => Brand.New(
        BrandId.New(),
        "Porsche",
        "Germany");

    public static Brand CreateInvalidTestBrand() => Brand.New(
        BrandId.New(),
        "",
        "");

    public static Brand UpdateValidTestBrand() => Brand.New(
        BrandId.New(),
        "Lexus",
        "Japan");

    public static Brand UpdateTakenTestBrand() => Brand.New(
        BrandId.New(),
        "Audi",
        "Germany");

    public static Brand UpdateInvalidTestBrand() => Brand.New(
        BrandId.New(),
        "",
        "");

    public static Guid NonExistentTestBrandId() => Guid.NewGuid();
}