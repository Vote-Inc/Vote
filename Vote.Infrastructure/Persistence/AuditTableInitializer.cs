using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Vote.Infrastructure.Persistence;

public sealed class AuditTableInitializer(
    [FromKeyedServices("audit")] IAmazonDynamoDB dynamoDb,
    ILogger<AuditTableInitializer> logger) : IHostedService
{
    private const string TableName = "evoting-audit";

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
            logger.LogError(ex, "Could not initialise '{Table}' table — is DynamoDB running?", TableName);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
