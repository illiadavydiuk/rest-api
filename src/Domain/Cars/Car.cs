namespace Domain.Cars;

public class Car
{
    public Guid Id { get; }
    public string Vin { get; private set; }
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public decimal Price { get; private set; }
    public FuelType FuelType { get; private set; }
    public bool IsAvailable { get; private set; }
    public DateTime CreatedAt { get; }

    private Car(Guid id, string vin, string brand, string model, decimal price, FuelType fuelType, bool isAvailable, DateTime createdAt)
        => (Id, Vin, Brand, Model, Price, FuelType, IsAvailable, CreatedAt) = (id, vin, brand, model, price, fuelType, isAvailable, createdAt);

    public static Car New(Guid id, string vin, string brand, string model, decimal price, FuelType fuelType, bool isAvailable)
        => new(id, vin, brand, model, price, fuelType, isAvailable, DateTime.UtcNow);

    public void UpdateDetails(string vin, string brand, string model, decimal price, FuelType fuelType, bool isAvailable)
        => (Vin, Brand, Model, Price, FuelType, IsAvailable) = (vin, brand, model, price, fuelType, isAvailable);
}