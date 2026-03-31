namespace Vote.Application.Common.Errors;

public static class VoteErrors
{
    public static readonly Error InvalidVoterId =
        new("vote.voter.invalid_id", "The provided voter ID is not valid.");

    public static readonly Error InvalidElectionId =
        new("vote.election.invalid_id", "The provided election ID is not valid.");

    public static readonly Error InvalidCandidateId =
        new("vote.candidate.invalid_id", "The provided candidate ID is not valid.");

    public static readonly Error AlreadyVoted =
        new("vote.voter.already_voted", "This voter has already cast a vote in this election.");
}