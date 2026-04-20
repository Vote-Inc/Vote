namespace Vote.API.Requests;

public sealed record CastVoteRequest(string VoterId, string ElectionId, string CandidateId);
