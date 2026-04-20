using Microsoft.Extensions.Logging;

namespace Vote.Infrastructure.Persistence;

public sealed class DynamoDbTableInitializer(
    IAmazonDynamoDB dynamoDb,
    ILogger<DynamoDbTableInitializer> logger) : IHostedService
{
    private const string TableName = "Votes";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        try
        {
            await dynamoDb.DescribeTableAsync(TableName, cts.Token);
        }
        catch (ResourceNotFoundException)
        {
            await dynamoDb.CreateTableAsync(new CreateTableRequest
            {
                TableName = TableName,
                AttributeDefinitions =
                [
                    new() { AttributeName = "VoterId",    AttributeType = ScalarAttributeType.S },
                    new() { AttributeName = "ElectionId", AttributeType = ScalarAttributeType.S }
                ],
                KeySchema =
                [
                    new() { AttributeName = "VoterId",    KeyType = KeyType.HASH  },
                    new() { AttributeName = "ElectionId", KeyType = KeyType.RANGE }
                ],
                BillingMode = BillingMode.PAY_PER_REQUEST
            }, cts.Token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not initialise '{Table}' table — is DynamoDB running?", TableName);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
