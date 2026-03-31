namespace Vote.Domain.Entities;

public sealed class Vote : BaseEntity
{
    public VoterId VoterId { get; private set; } = null!;
    public ElectionId ElectionId { get; private set; } = null!;
    public CandidateId CandidateId { get; private set; } = null!;
    
    private Vote() { }
    
    public static Vote Create(VoterId voterId, ElectionId electionId, CandidateId candidateId)
    {
        ArgumentNullException.ThrowIfNull(voterId);
        ArgumentNullException.ThrowIfNull(electionId);
        ArgumentNullException.ThrowIfNull(candidateId);

        var vote = new Vote
        {
            VoterId = voterId,
            ElectionId = electionId,
            CandidateId = candidateId,
        };

        vote.RaiseDomainEvent(new VoteCastEvent(voterId, electionId, candidateId));
        return vote;
    }
}