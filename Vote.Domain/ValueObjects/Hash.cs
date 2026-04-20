namespace Vote.Domain.ValueObjects;

public sealed class Hash : ValueObject
{
    public string Value { get; }

    private Hash(string value) => Value = value;

    public static Hash Of(string plaintext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plaintext);
        var hex = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(plaintext)));
        return new Hash(hex);
    }

    public static Hash FromHex(string hex)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hex);
        return new Hash(hex);
    }

    public static Hash Zero { get; } = new(new string('0', 64));

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
