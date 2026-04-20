namespace Vote.Domain.Exceptions;

public sealed class DuplicateVoteException: DomainException
{
    public DuplicateVoteException(VoterId voterId)
        : base($"Voter '{voterId.Value}' has already voted.") { }
}