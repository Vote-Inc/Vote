namespace Vote.Domain.Interfaces.Repositories;

public interface IVoteRepository
{
    Task AddAsync(Entities.Vote vote, CancellationToken ct = default);
    Task<bool> ExistsAsync(VoterId voterId, ElectionId electionId, CancellationToken ct = default);
}