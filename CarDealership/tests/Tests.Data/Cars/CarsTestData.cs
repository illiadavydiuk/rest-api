using Domain.Brands;
using Domain.Cars;

namespace Tests.Data.Cars;

public static class CarsTestData
{
    public static Car CreateValidTestCar(BrandId brandId) => Car.New(
        CarId.New(),
        "JN1FAAY34U0098771",
        brandId,
        "Skyline GT-R R34",
        125000.00m,
        FuelType.Gasoline,
        true);
    
    public static Car CreateTakenTestCar(BrandId brandId) => Car.New(
        CarId.New(),
        "5YJYGDEE8MF123456",
        brandId,
        "Model Y Long Range",
        42500.00m,
        FuelType.Electric,
        true);
    
    public static Car CreateInvalidTestCar(BrandId brandId) => Car.New(
        CarId.New(),
        "", 
        brandId,
        "InvalidModel",
        -500.00m, 
        FuelType.Gasoline,
        true);
    
    public static Car UpdateValidTestCar(BrandId brandId) => Car.New(
        CarId.New(),
        "WBA51BJ080G987654",
        brandId,
        "530d xDrive",
        38000.00m,
        FuelType.Diesel,
        true);
    
    public static Car UpdateTakenTestCar(BrandId brandId) => Car.New(
        CarId.New(),
        "JTMDWRFV50D456789",
        brandId,
        "RAV4 Hybrid",
        31500.00m,
        FuelType.Hybrid,
        true);
    
    public static Car UpdateInvalidTestCar(BrandId brandId) => Car.New(
        CarId.New(),
        "", 
        brandId,
        "BadModel",
        -100.00m, 
        FuelType.Gasoline,
        false);
    
    public static Guid NonExistentTestCarId() => Guid.NewGuid();
}