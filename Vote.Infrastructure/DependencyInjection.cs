namespace Vote.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IAmazonDynamoDB>(_ =>
        {
            var serviceUrl = configuration["DynamoDB:ServiceUrl"];
            var config = new AmazonDynamoDBConfig();

            if (!string.IsNullOrWhiteSpace(serviceUrl))
            {
                config.ServiceURL = serviceUrl;
                return new AmazonDynamoDBClient(
                    new Amazon.Runtime.BasicAWSCredentials("dummy", "dummy"),
                    config);
            }

            return new AmazonDynamoDBClient(config);
        });

        services.AddHostedService<DynamoDbTableInitializer>();
        services.AddScoped<IVoteRepository, VoteRepository>();
        services.AddScoped<IUnitOfWork, DynamoDbUnitOfWork>();

        return services;
    }
}
