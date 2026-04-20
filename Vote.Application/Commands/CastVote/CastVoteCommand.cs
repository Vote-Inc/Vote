namespace Vote.Application.Commands.CastVote;

public sealed record CastVoteCommand(
    string VoterId,
    string ElectionId,
    string CandidateId);