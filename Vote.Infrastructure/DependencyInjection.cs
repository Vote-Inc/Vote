namespace Vote.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IAmazonDynamoDB>(_ =>
            BuildDynamoClient(configuration["DynamoDB:ServiceUrl"], configuration["DynamoDB:Region"]));

        services.AddKeyedSingleton<IAmazonDynamoDB>("audit", (_, _) =>
            BuildDynamoClient(configuration["AuditDynamoDB:ServiceUrl"], configuration["AuditDynamoDB:Region"]));

        services.AddHostedService<DynamoDbTableInitializer>();
        services.AddHostedService<AuditTableInitializer>();

        services.AddScoped<IVoteRepository,  VoteRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IUnitOfWork,      DynamoDbUnitOfWork>();

        return services;
    }

    private static IAmazonDynamoDB BuildDynamoClient(string? serviceUrl, string? region = null)
    {
        var config = new AmazonDynamoDBConfig();

        if (!string.IsNullOrWhiteSpace(serviceUrl))
        {
            config.ServiceURL = serviceUrl;
            return new AmazonDynamoDBClient(
                new Amazon.Runtime.BasicAWSCredentials("dummy", "dummy"),
                config);
        }

        if (!string.IsNullOrWhiteSpace(region))
            config.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region);

        return new AmazonDynamoDBClient(config);
    }
}
