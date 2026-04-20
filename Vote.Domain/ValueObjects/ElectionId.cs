namespace Vote.Domain.ValueObjects;

public sealed partial class ElectionId: ValueObject
{
    public string Value { get; }
    
    private ElectionId(string value) => Value = value;

    public static ElectionId Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidElectionIdException(value);
        
        return new ElectionId(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}