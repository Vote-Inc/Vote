namespace Vote.Application.Commands.CastVote;

public sealed class CastVoteHandler(
    IVoteRepository  voteRepository,
    IUnitOfWork      unitOfWork,
    IAuditRepository auditRepository)
{
    public async Task<Result<string>> Handle(
        CastVoteCommand command,
        CancellationToken ct = default)
    {
        VoterId     voterId;
        ElectionId  electionId;
        CandidateId candidateId;
        try
        {
            voterId     = VoterId.Create(command.VoterId);
            electionId  = ElectionId.Create(command.ElectionId);
            candidateId = CandidateId.Create(command.CandidateId);
        }
        catch (InvalidVoterIdException)
        {
            return Result<string>.Failure(VoteErrors.InvalidVoterId);
        }
        catch (InvalidElectionIdException)
        {
            return Result<string>.Failure(VoteErrors.InvalidElectionId);
        }
        catch (InvalidCandidateIdException)
        {
            return Result<string>.Failure(VoteErrors.InvalidCandidateId);
        }

        var hasVoted = await voteRepository.ExistsAsync(voterId, electionId, ct);

        if (hasVoted)
            return Result<string>.Failure(VoteErrors.AlreadyVoted);

        var vote = Domain.Entities.Vote.Create(voterId, electionId, candidateId);

        await voteRepository.AddAsync(vote, ct);
        await unitOfWork.CommitAsync(ct);

        var voterHash = Hash.Of(command.VoterId);

        var entry = await auditRepository.AppendAsync(electionId, voterHash, candidateId, ct);

        return Result<string>.Success(entry.ReceiptId.ToString());
    }
}
