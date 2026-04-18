namespace Vote.API.Controllers;

[ApiController]
[Route("vote")]
public sealed class VoteController(
    CastVoteHandler castVoteHandler
    ) : ControllerBase
{
    [HttpPost("")]
    public async Task<IActionResult> Vote(
        [FromBody] VoteRequest request,
        CancellationToken ct)
    {
        var result = await castVoteHandler.Handle(
            new CastVoteCommand(request.VoterId, request.ElectionId, request.CandidateId), ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: error => error.Code switch
            {
                "vote.voter.invalid_id" or
                "vote.election.invalid_id" or
                "vote.candidate.invalid_id"   => Unauthorized(error.ToResponse()),
                "vote.voter.already_voted"    => Conflict(error.ToResponse()),
                _                             => BadRequest(error.ToResponse())
            });
    }
}
public sealed record VoteRequest(string VoterId, string ElectionId, string CandidateId);