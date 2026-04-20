namespace Vote.Domain.Entities;

public sealed class AuditEntry : BaseEntity
{
    public ElectionId  ElectionId  { get; private set; } = null!;
    public int         Version     { get; private set; }
    public Hash        VoterHash   { get; private set; } = null!;
    public CandidateId CandidateId { get; private set; } = null!;
    public DateTime    Timestamp   { get; private set; }
    public Guid        ReceiptId   { get; private set; }
    public Hash        PrevHash    { get; private set; } = null!;
    public Hash        EntryHash   { get; private set; } = null!;

    private AuditEntry() { }
    
    public static AuditEntry Create(
        ElectionId  electionId,
        int         version,
        Hash        voterHash,
        CandidateId candidateId,
        Hash        prevHash)
    {
        ArgumentNullException.ThrowIfNull(electionId);
        ArgumentNullException.ThrowIfNull(voterHash);
        ArgumentNullException.ThrowIfNull(candidateId);
        ArgumentNullException.ThrowIfNull(prevHash);
        ArgumentOutOfRangeException.ThrowIfLessThan(version, 1);

        var receiptId = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;

        var content   = $"{electionId.Value}:{voterHash.Value}:{candidateId.Value}:{timestamp:O}:{prevHash.Value}";
        var entryHash = Hash.Of(content);

        var entry = new AuditEntry
        {
            ElectionId  = electionId,
            Version     = version,
            VoterHash   = voterHash,
            CandidateId = candidateId,
            Timestamp   = timestamp,
            ReceiptId   = receiptId,
            PrevHash    = prevHash,
            EntryHash   = entryHash
        };

        entry.RaiseDomainEvent(new AuditEntryRecordedEvent(
            electionId, version, voterHash, candidateId, receiptId));

        return entry;
    }
    
    public static AuditEntry Reconstitute(
        string electionId,
        int    version,
        string voterHash,
        string candidateId,
        string timestamp,
        string receiptId,
        string prevHash,
        string entryHash) =>
        new()
        {
            ElectionId  = ElectionId.Create(electionId),
            Version     = version,
            VoterHash   = Hash.FromHex(voterHash),
            CandidateId = CandidateId.Create(candidateId),
            Timestamp   = DateTime.Parse(timestamp, null, System.Globalization.DateTimeStyles.RoundtripKind),
            ReceiptId   = Guid.Parse(receiptId),
            PrevHash    = Hash.FromHex(prevHash),
            EntryHash   = Hash.FromHex(entryHash)
        };
}
