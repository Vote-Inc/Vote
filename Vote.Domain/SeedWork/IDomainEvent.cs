namespace Vote.Domain.SeedWork;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}