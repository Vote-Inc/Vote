namespace Vote.Domain.SeedWork;

public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        return ((ValueObject)obj)
            .GetEqualityComponents()
            .SequenceEqual(GetEqualityComponents());
    }

    public override int GetHashCode() =>
        GetEqualityComponents()
            .Aggregate(default(int), HashCode.Combine);

    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        Equals(left, right);

    public static bool operator !=(ValueObject? left, ValueObject? right) =>
        !Equals(left, right);
}