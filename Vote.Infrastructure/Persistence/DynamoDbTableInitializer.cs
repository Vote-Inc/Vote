namespace Vote.Infrastructure.Persistence;

public sealed class DynamoDbTableInitializer(
    IAmazonDynamoDB dynamoDb,
    IConfiguration configuration,
    ILogger<DynamoDbTableInitializer> logger) : IHostedService
{
    private readonly string _tableName = configuration["DynamoDB:TableName"] ?? "evoting-votes";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
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
                    new() { AttributeName = "voterHash",  AttributeType = ScalarAttributeType.S }
                ],
                KeySchema =
                [
                    new() { AttributeName = "electionId", KeyType = KeyType.HASH  },
                    new() { AttributeName = "voterHash",  KeyType = KeyType.RANGE }
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
