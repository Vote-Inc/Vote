namespace Vote.Domain.Exceptions;

public sealed class InvalidElectionIdException : DomainException
{
    public InvalidElectionIdException(string electionId)
        : base($"'{electionId}' is not a valid election id.") { }
}