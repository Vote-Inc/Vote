namespace Vote.Infrastructure.Persistence;

public sealed class DynamoDbTableInitializer(IAmazonDynamoDB dynamoDb) : IHostedService
{
    private const string TableName = "Votes";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dynamoDb.DescribeTableAsync(TableName, cancellationToken);
        }
        catch (ResourceNotFoundException)
        {
            await dynamoDb.CreateTableAsync(new CreateTableRequest
            {
                TableName = TableName,
                AttributeDefinitions =
                [
                    new AttributeDefinition { AttributeName = "VoterId",    AttributeType = ScalarAttributeType.S },
                    new AttributeDefinition { AttributeName = "ElectionId", AttributeType = ScalarAttributeType.S }
                ],
                KeySchema =
                [
                    new KeySchemaElement { AttributeName = "VoterId",    KeyType = KeyType.HASH  },
                    new KeySchemaElement { AttributeName = "ElectionId", KeyType = KeyType.RANGE }
                ],
                BillingMode = BillingMode.PAY_PER_REQUEST
            }, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
