namespace Vote.API.Controllers;

[ApiController]
[Route("api/votes")]
public sealed class VoteController(
    CastVoteHandler       castVoteHandler,
    GetVoteReceiptHandler getVoteReceiptHandler) : ControllerBase
{
    [HttpPost("")]
    public async Task<IActionResult> CastVote(
        [FromBody] CastVoteRequest request,
        CancellationToken ct)
    {
        var voterId = HttpContext.Request.Headers["X-Voter-Id"].FirstOrDefault();

        var result = await castVoteHandler.Handle(
            new CastVoteCommand(voterId ?? string.Empty, request.ElectionId, request.CandidateId), ct);

        return result.Match<IActionResult>(
            onSuccess: receiptId => Ok(new { receiptId }),
            onFailure: error => error.Code switch
            {
                "vote.voter.invalid_id"    or
                "vote.election.invalid_id" or
                "vote.candidate.invalid_id"  => Unauthorized(error.ToResponse()),
                "vote.voter.already_voted"   => Conflict(error.ToResponse()),
                _                            => BadRequest(error.ToResponse())
            });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyRequest request, CancellationToken ct)
    {
        var result = await getVoteReceiptHandler.Handle(new GetVoteReceiptQuery(request.ReceiptId), ct);

        return result.Match<IActionResult>(
            onSuccess: entry => Ok(new
            {
                electionId  = entry.ElectionId.Value,
                candidateId = entry.CandidateId.Value,
                timestamp   = entry.Timestamp,
                verified    = true
            }),
            onFailure: error => error.Code switch
            {
                "vote.receipt.not_found" => NotFound(error.ToResponse()),
                _                        => BadRequest(error.ToResponse())
            });
    }
}

