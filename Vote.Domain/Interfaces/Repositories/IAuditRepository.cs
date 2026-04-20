namespace Vote.Domain.Interfaces.Repositories;

public interface IAuditRepository
{
    /// <summary>Appends a new chained entry and returns it (including the generated ReceiptId).</summary>
    Task<AuditEntry> AppendAsync(
        ElectionId  electionId,
        Hash        voterHash,
        CandidateId candidateId,
        CancellationToken ct = default);

    Task<AuditEntry?> GetByReceiptIdAsync(Guid receiptId, CancellationToken ct = default);
}
