using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSolve.Api.DTOs;
using UniSolve.Api.Services.Interfaces;

namespace UniSolve.Api.Controllers;

[ApiController]
[Route("api")]
public class SolutionsController : ControllerBase
{
    private readonly ISolutionService _solutionService;

    public SolutionsController(ISolutionService solutionService)
    {
        _solutionService = solutionService;
    }

    [Authorize(Roles = "Verified,Moderator,Admin")]
    [HttpPost("problems/{problemId}/solutions")]
    public async Task<ActionResult<SolutionDto>> Create(int problemId, CreateSolutionRequest request)
    {
        var solution = await _solutionService.CreateAsync(problemId, request, User.GetUserId());
        if (solution == null)
            return NotFound(new { message = "Problem not found." });

        return CreatedAtAction(nameof(Create), new { id = solution.Id }, solution);
    }

    [Authorize(Roles = "Verified,Moderator,Admin")]
    [HttpPut("solutions/{id}")]
    public async Task<ActionResult<SolutionDto>> Update(int id, UpdateSolutionRequest request)
    {
        var isModerator = User.IsInAnyRole("Moderator", "Admin");
        var solution = await _solutionService.UpdateAsync(id, request, User.GetUserId(), isModerator);
        if (solution == null) return NotFound();
        return Ok(solution);
    }

    [Authorize(Roles = "Verified,Moderator,Admin")]
    [HttpDelete("solutions/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var isModerator = User.IsInAnyRole("Moderator", "Admin");
        var deleted = await _solutionService.DeleteAsync(id, User.GetUserId(), isModerator);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
