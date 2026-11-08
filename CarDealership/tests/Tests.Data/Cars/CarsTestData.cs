using Domain.Cars;

namespace Tests.Data.Cars;

public static class CarsTestData
{
    public static Car CreateValidTestCar() => Car.New(
        CarId.New(),
        "JN1FAAY34U0098771",
        "Nissan",
        "Skyline GT-R R34",
        125000.00m,
        FuelType.Gasoline,
        true);
    
    public static Car CreateTakenTestCar() => Car.New(
        CarId.New(),
        "5YJYGDEE8MF123456",
        "Tesla",
        "Model Y Long Range",
        42500.00m,
        FuelType.Electric,
        true);
    
    public static Car CreateInvalidTestCar() => Car.New(
        CarId.New(),
        "", // Порожній VIN
        "InvalidBrand",
        "InvalidModel",
        -500.00m, // Від'ємна ціна
        FuelType.Gasoline,
        true);
    
    public static Car UpdateValidTestCar() => Car.New(
        CarId.New(),
        "WBA51BJ080G987654",
        "BMW",
        "530d xDrive",
        38000.00m,
        FuelType.Diesel,
        true);
    
    public static Car UpdateTakenTestCar() => Car.New(
        CarId.New(),
        "JTMDWRFV50D456789",
        "Toyota",
        "RAV4 Hybrid",
        31500.00m,
        FuelType.Hybrid,
        true);
    
    public static Car UpdateInvalidTestCar() => Car.New(
        CarId.New(),
        "", 
        "BadBrand",
        "BadModel",
        -100.00m,
        FuelType.Gasoline,
        false);
    
    public static Guid NonExistentTestCarId() => Guid.NewGuid();
}