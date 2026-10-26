namespace Domain.Tracks;

public record TrackId(Guid Value)
{
    public static TrackId Empty() => new(Guid.Empty);
    public static TrackId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}