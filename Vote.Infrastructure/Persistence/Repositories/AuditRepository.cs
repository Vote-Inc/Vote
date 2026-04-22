namespace Vote.Infrastructure.Persistence.Repositories;

public sealed class AuditRepository(
    [FromKeyedServices("audit")] IAmazonDynamoDB dynamoDb,
    IConfiguration configuration) : IAuditRepository
{
    private readonly string _tableName = configuration["AuditDynamoDB:TableName"] ?? "evoting-audit";

    public async Task<AuditEntry> AppendAsync(
        ElectionId  electionId,
        Hash        voterHash,
        CandidateId candidateId,
        CancellationToken ct = default)
    {
        var lastEntry = await GetLatestEntryAsync(electionId.Value, ct);
        var version   = (lastEntry?.Version ?? 0) + 1;
        var prevHash  = lastEntry?.EntryHash ?? Hash.Zero;

        var entry = AuditEntry.Create(electionId, version, voterHash, candidateId, prevHash);

        await dynamoDb.PutItemAsync(new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                ["electionId"]  = new() { S = entry.ElectionId.Value },
                ["version"]     = new() { N = entry.Version.ToString() },
                ["voterHash"]   = new() { S = entry.VoterHash.Value },
                ["candidateId"] = new() { S = entry.CandidateId.Value },
                ["timestamp"]   = new() { S = entry.Timestamp.ToString("O") },
                ["receiptId"]   = new() { S = entry.ReceiptId.ToString() },
                ["prevHash"]    = new() { S = entry.PrevHash.Value },
                ["entryHash"]   = new() { S = entry.EntryHash.Value }
            },
            ConditionExpression = "attribute_not_exists(version)"
        }, ct);

        return entry;
    }

    public async Task<AuditEntry?> GetByReceiptIdAsync(Guid receiptId, CancellationToken ct = default)
    {
        var response = await dynamoDb.QueryAsync(new QueryRequest
        {
            TableName              = _tableName,
            IndexName              = "receiptId-index",
            KeyConditionExpression = "receiptId = :r",
            ExpressionAttributeValues = new()
            {
                [":r"] = new AttributeValue { S = receiptId.ToString() }
            }
        }, ct);

        return response.Items.Count == 0
            ? null
            : MapToAuditEntry(response.Items[0]);
    }

    private async Task<AuditEntry?> GetLatestEntryAsync(string electionId, CancellationToken ct)
    {
        var response = await dynamoDb.QueryAsync(new QueryRequest
        {
            TableName              = _tableName,
            KeyConditionExpression = "electionId = :e",
            ExpressionAttributeValues = new()
            {
                [":e"] = new AttributeValue { S = electionId }
            },
            ScanIndexForward = false,
            Limit = 1
        }, ct);

        return response.Items.Count == 0
            ? null
            : MapToAuditEntry(response.Items[0]);
    }

    private static AuditEntry MapToAuditEntry(Dictionary<string, AttributeValue> item) =>
        AuditEntry.Reconstitute(
            electionId:  item["electionId"].S,
            version:     int.Parse(item["version"].N),
            voterHash:   item["voterHash"].S,
            candidateId: item["candidateId"].S,
            timestamp:   item["timestamp"].S,
            receiptId:   item["receiptId"].S,
            prevHash:    item["prevHash"].S,
            entryHash:   item["entryHash"].S
        );
}
