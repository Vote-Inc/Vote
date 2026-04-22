namespace Vote.Infrastructure.Persistence;

public sealed class AuditTableInitializer(
    [FromKeyedServices("audit")] IAmazonDynamoDB dynamoDb,
    IConfiguration configuration,
    ILogger<AuditTableInitializer> logger) : IHostedService
{
    private readonly string _tableName = configuration["AuditDynamoDB:TableName"] ?? "evoting-audit";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        try
        {
            await dynamoDb.DescribeTableAsync(_tableName, cts.Token);
        }
        catch (ResourceNotFoundException)
        {
            await dynamoDb.CreateTableAsync(new CreateTableRequest
            {
                TableName = _tableName,
                AttributeDefinitions =
                [
                    new() { AttributeName = "electionId", AttributeType = ScalarAttributeType.S },
                    new() { AttributeName = "version",    AttributeType = ScalarAttributeType.N },
                    new() { AttributeName = "receiptId",  AttributeType = ScalarAttributeType.S }
                ],
                KeySchema =
                [
                    new() { AttributeName = "electionId", KeyType = KeyType.HASH  },
                    new() { AttributeName = "version",    KeyType = KeyType.RANGE }
                ],
                GlobalSecondaryIndexes =
                [
                    new()
                    {
                        IndexName = "receiptId-index",
                        KeySchema =
                        [
                            new() { AttributeName = "receiptId", KeyType = KeyType.HASH }
                        ],
                        Projection = new() { ProjectionType = ProjectionType.ALL }
                    }
                ],
                BillingMode = BillingMode.PAY_PER_REQUEST
            }, cts.Token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not initialise '{Table}' table — is DynamoDB running?", _tableName);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
