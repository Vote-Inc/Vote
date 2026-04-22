namespace Vote.Infrastructure.Persistence.Repositories;

public sealed class VoteRepository(IAmazonDynamoDB dynamoDb, IConfiguration configuration) : IVoteRepository
{
    private readonly string _tableName = configuration["DynamoDB:TableName"] ?? "evoting-votes";

    public async Task AddAsync(Domain.Entities.Vote vote, CancellationToken ct = default)
    {
        var request = new PutItemRequest
        {
            TableName = _tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                ["electionId"]  = new() { S = vote.ElectionId.Value },
                ["voterHash"]   = new() { S = Hash.Of(vote.VoterId.Value).Value },
                ["candidateId"] = new() { S = vote.CandidateId.Value }
            },
            ConditionExpression = "attribute_not_exists(voterHash)"
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
            TableName = _tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["electionId"] = new() { S = electionId.Value },
                ["voterHash"]  = new() { S = Hash.Of(voterId.Value).Value }
            },
            ProjectionExpression = "electionId"
        };

        var response = await dynamoDb.GetItemAsync(request, ct);
        return response.IsItemSet;
    }
}
