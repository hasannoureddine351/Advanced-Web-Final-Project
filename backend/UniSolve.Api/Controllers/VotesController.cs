using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSolve.Api.DTOs;
using UniSolve.Api.Services.Interfaces;

namespace UniSolve.Api.Controllers;

[ApiController]
[Route("api/solutions/{id}/vote")]
public class VotesController : ControllerBase
{
    private readonly IVoteService _voteService;

    public VotesController(IVoteService voteService)
    {
        _voteService = voteService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SolutionDto>> Vote(int id, VoteRequest request)
    {
        var result = await _voteService.VoteAsync(id, request.Value, User.GetUserId());
        if (result == null)
            return BadRequest(new { message = "Cannot vote on this solution." });

        return Ok(result);
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult<SolutionDto>> RemoveVote(int id)
    {
        var result = await _voteService.RemoveVoteAsync(id, User.GetUserId());
        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
