namespace Vote.Domain.Exceptions;

public sealed class InvalidVoterIdException : DomainException
{
    public InvalidVoterIdException(string voterId)
        : base($"'{voterId}' is not a valid voter id.") { }
}