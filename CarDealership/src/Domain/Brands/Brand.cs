namespace Domain.Brands;

public class Brand
{
    public BrandId Id { get; }
    public string Name { get; private set; }
    public string Country { get; private set; }
    public DateTime CreatedAt { get; }

    private Brand(BrandId id, string name, string country, DateTime createdAt)
        => (Id, Name, Country, CreatedAt) = (id, name, country, createdAt);

    public static Brand New(BrandId id, string name, string country)
        => new(id, name, country, DateTime.UtcNow);

    public void UpdateDetails(string name, string country)
        => (Name, Country) = (name, country);
}