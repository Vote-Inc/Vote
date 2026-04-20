namespace Vote.Domain.Exceptions;

public sealed class InvalidCandidateIdException : DomainException
{
    public InvalidCandidateIdException(string candidateId)
        : base($"'{candidateId}' is not a valid candidate id.") { }
}