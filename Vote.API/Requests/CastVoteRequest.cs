namespace Vote.API.Requests;

public sealed record CastVoteRequest(string ElectionId, string CandidateId);
