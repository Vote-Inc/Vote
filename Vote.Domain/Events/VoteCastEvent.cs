namespace Vote.Domain.Events;

public sealed record VoteCastEvent(
    VoterId VoterId,
    ElectionId ElectionId,
    CandidateId CandidateId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}