namespace Vote.Infrastructure.Persistence.Repositories;

public sealed class DynamoDbUnitOfWork : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken ct = default)
        => Task.FromResult(0);
}
