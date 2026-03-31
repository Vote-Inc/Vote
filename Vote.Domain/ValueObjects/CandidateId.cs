namespace Vote.Domain.ValueObjects;

public sealed partial class CandidateId: ValueObject
{
    public string Value { get; }
    
    private CandidateId(string value) => Value = value;

    public static CandidateId Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidCandidateIdException(value);
        
        return new CandidateId(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}