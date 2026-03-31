namespace Vote.Application.Commands.CastVote;

public sealed class CastVoteHandler(
    IVoteRepository voteRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<Result> Handle(
        CastVoteCommand command,
        CancellationToken ct = default)
    {
        VoterId voterId;
        ElectionId electionId;
        CandidateId candidateId;
        try
        {
            voterId = VoterId.Create(command.VoterId);
            electionId = ElectionId.Create(command.ElectionId);
            candidateId = CandidateId.Create(command.CandidateId);
        }
        catch (InvalidVoterIdException)
        {
            return Result.Failure(VoteErrors.InvalidVoterId);
        }
        catch (InvalidElectionIdException)
        {
            return Result.Failure(VoteErrors.InvalidElectionId);
        }
        catch (InvalidCandidateIdException)
        {
            return Result.Failure(VoteErrors.InvalidCandidateId);
        }

        var hasVoted = await voteRepository.ExistsAsync(voterId, electionId, ct);
        
        if (hasVoted)
            return Result.Failure(VoteErrors.AlreadyVoted);

        var vote = Domain.Entities.Vote.Create(
            voterId,
            electionId,
            candidateId);

        await voteRepository.AddAsync(vote, ct);
        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}