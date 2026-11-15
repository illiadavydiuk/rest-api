using Domain.Brands;

namespace Domain.Cars;

public class Car
{
    public CarId Id { get; }
    public string Vin { get; private set; }
    public BrandId BrandId { get; private set; }
    public string Model { get; private set; }
    public decimal Price { get; private set; }
    public FuelType FuelType { get; private set; }
    public bool IsAvailable { get; private set; }
    public DateTime CreatedAt { get; }
    
    public Brand Brand { get; } = null;

    private Car(CarId id, string vin, BrandId brandId, string model, decimal price, FuelType fuelType, bool isAvailable, DateTime createdAt)
        => (Id, Vin, BrandId, Model, Price, FuelType, IsAvailable, CreatedAt) = (id, vin, brandId, model, price, fuelType, isAvailable, createdAt);

    public static Car New(CarId id, string vin, BrandId brandId, string model, decimal price, FuelType fuelType, bool isAvailable)
        => new(id, vin, brandId, model, price, fuelType, isAvailable, DateTime.UtcNow);

    public void UpdateDetails(string vin, BrandId brandId, string model, decimal price, FuelType fuelType, bool isAvailable)
        => (Vin, BrandId, Model, Price, FuelType, IsAvailable) = (vin, brandId, model, price, fuelType, isAvailable);
}