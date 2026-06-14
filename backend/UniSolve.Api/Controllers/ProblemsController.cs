using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSolve.Api.DTOs;
using UniSolve.Api.Services.Interfaces;

namespace UniSolve.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProblemsController : ControllerBase
{
    private readonly IProblemService _problemService;

    public ProblemsController(IProblemService problemService)
    {
        _problemService = problemService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProblemListItemDto>>> GetAll(
        [FromQuery] int? subjectId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);
        return Ok(await _problemService.GetAllAsync(subjectId, search, page, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProblemDetailDto>> GetById(int id)
    {
        int? userId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        var problem = await _problemService.GetByIdAsync(id, userId);
        if (problem == null) return NotFound();
        return Ok(problem);
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<List<ProblemListItemDto>>> GetMine()
    {
        return Ok(await _problemService.GetMineAsync(User.GetUserId()));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProblemDetailDto>> Create(CreateProblemRequest request)
    {
        var problem = await _problemService.CreateAsync(request, User.GetUserId());
        if (problem == null)
            return BadRequest(new { message = "Invalid subject." });

        return CreatedAtAction(nameof(GetById), new { id = problem.Id }, problem);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ProblemDetailDto>> Update(int id, UpdateProblemRequest request)
    {
        var isAdmin = User.IsInRole("Admin");
        var problem = await _problemService.UpdateAsync(id, request, User.GetUserId(), isAdmin);
        if (problem == null)
            return NotFound();

        return Ok(problem);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var isAdmin = User.IsInRole("Admin");
        var deleted = await _problemService.DeleteAsync(id, User.GetUserId(), isAdmin);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
