namespace Vote.Domain.Events;

public sealed record AuditEntryRecordedEvent(
    ElectionId  ElectionId,
    int         Version,
    Hash        VoterHash,
    CandidateId CandidateId,
    Guid        ReceiptId) : IDomainEvent
{
    public Guid     EventId    { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
