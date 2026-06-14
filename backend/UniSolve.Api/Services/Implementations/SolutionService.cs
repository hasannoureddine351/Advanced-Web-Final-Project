using Microsoft.EntityFrameworkCore;
using UniSolve.Api.Data;
using UniSolve.Api.DTOs;
using UniSolve.Api.Models.Entities;
using UniSolve.Api.Services.Interfaces;

namespace UniSolve.Api.Services.Implementations;

public class SolutionService : ISolutionService
{
    private readonly ApplicationDbContext _context;

    public SolutionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SolutionDto?> CreateAsync(int problemId, CreateSolutionRequest request, int userId)
    {
        if (!await _context.Problems.AnyAsync(p => p.Id == problemId))
            return null;

        var solution = new Solution
        {
            ProblemId = problemId,
            AuthorId = userId,
            Content = request.Content.Trim()
        };

        _context.Solutions.Add(solution);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(solution.Id, userId);
    }

    public async Task<SolutionDto?> UpdateAsync(int id, UpdateSolutionRequest request, int userId, bool isModerator)
    {
        var solution = await _context.Solutions.FindAsync(id);
        if (solution == null) return null;
        if (solution.AuthorId != userId && !isModerator) return null;

        solution.Content = request.Content.Trim();
        solution.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return await GetByIdAsync(id, userId);
    }

    public async Task<bool> DeleteAsync(int id, int userId, bool isModerator)
    {
        var solution = await _context.Solutions.FindAsync(id);
        if (solution == null) return false;
        if (solution.AuthorId != userId && !isModerator) return false;

        _context.Solutions.Remove(solution);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<SolutionDto?> GetByIdAsync(int id, int? currentUserId)
    {
        var solution = await _context.Solutions
            .Include(s => s.Author)
            .Include(s => s.Votes)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (solution == null) return null;

        var upvotes = solution.Votes.Count(v => v.Value == 1);
        var downvotes = solution.Votes.Count(v => v.Value == -1);

        return new SolutionDto
        {
            Id = solution.Id,
            ProblemId = solution.ProblemId,
            AuthorId = solution.AuthorId,
            AuthorUsername = solution.Author.Username,
            Content = solution.Content,
            Score = solution.Score,
            Upvotes = upvotes,
            Downvotes = downvotes,
            UserVote = currentUserId.HasValue
                ? solution.Votes.FirstOrDefault(v => v.UserId == currentUserId.Value)?.Value
                : null,
            CreatedAt = solution.CreatedAt,
            UpdatedAt = solution.UpdatedAt
        };
    }
}
