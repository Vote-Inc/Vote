namespace Vote.Domain.ValueObjects;

public sealed partial class VoterId : ValueObject
{
    public string Value { get; }
    
    private VoterId(string value) => Value = value;

    public static VoterId Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidVoterIdException(value);
        
        return new VoterId(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}