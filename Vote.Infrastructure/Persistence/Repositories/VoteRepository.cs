namespace Vote.Infrastructure.Persistence.Repositories;

public sealed class VoteRepository(IAmazonDynamoDB dynamoDb) : IVoteRepository
{
    private const string TableName = "Votes";

    public async Task AddAsync(Domain.Entities.Vote vote, CancellationToken ct = default)
    {
        var request = new PutItemRequest
        {
            TableName = TableName,
            Item = new Dictionary<string, AttributeValue>
            {
                ["VoterId"]    = new() { S = vote.VoterId.Value },
                ["ElectionId"] = new() { S = vote.ElectionId.Value },
                ["CreatedAt"]  = new() { S = vote.CreatedAt.ToString("O") }
            },
            ConditionExpression = "attribute_not_exists(VoterId)"
        };

        await dynamoDb.PutItemAsync(request, ct);
    }

    public async Task<bool> ExistsAsync(
        VoterId voterId,
        ElectionId electionId,
        CancellationToken ct = default)
    {
        var request = new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["VoterId"]    = new() { S = voterId.Value },
                ["ElectionId"] = new() { S = electionId.Value }
            },
            ProjectionExpression = "VoterId"
        };

        var response = await dynamoDb.GetItemAsync(request, ct);
        return response.IsItemSet;
    }
}
