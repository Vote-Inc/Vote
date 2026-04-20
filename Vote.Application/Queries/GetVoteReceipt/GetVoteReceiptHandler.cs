namespace Vote.Application.Queries.GetVoteReceipt;

public sealed class GetVoteReceiptHandler(IAuditRepository auditRepository)
{
    public async Task<Result<AuditEntry>> Handle(
        GetVoteReceiptQuery query,
        CancellationToken ct = default)
    {
        if (!Guid.TryParse(query.ReceiptId, out var receiptGuid))
            return Result<AuditEntry>.Failure(VoteErrors.ReceiptNotFound);

        var entry = await auditRepository.GetByReceiptIdAsync(receiptGuid, ct);

        return entry is null
            ? Result<AuditEntry>.Failure(VoteErrors.ReceiptNotFound)
            : Result<AuditEntry>.Success(entry);
    }
}
