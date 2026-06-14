using Microsoft.EntityFrameworkCore;
using UniSolve.Api.Data;
using UniSolve.Api.DTOs;
using UniSolve.Api.Models.Entities;
using UniSolve.Api.Services.Interfaces;

namespace UniSolve.Api.Services.Implementations;

public class VoteService : IVoteService
{
    private readonly ApplicationDbContext _context;

    public VoteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SolutionDto?> VoteAsync(int solutionId, int value, int userId)
    {
        if (value is not (1 or -1))
            return null;

        var solution = await _context.Solutions
            .Include(s => s.Author)
            .Include(s => s.Votes)
            .FirstOrDefaultAsync(s => s.Id == solutionId);

        if (solution == null) return null;
        if (solution.AuthorId == userId) return null;

        var existingVote = solution.Votes.FirstOrDefault(v => v.UserId == userId);

        if (existingVote != null)
        {
            if (existingVote.Value == value)
            {
                _context.Votes.Remove(existingVote);
            }
            else
            {
                existingVote.Value = value;
            }
        }
        else
        {
            _context.Votes.Add(new Vote
            {
                SolutionId = solutionId,
                UserId = userId,
                Value = value
            });
        }

        await _context.SaveChangesAsync();
        await RecalculateScoreAsync(solutionId);
        return await GetSolutionDtoAsync(solutionId, userId);
    }

    public async Task<SolutionDto?> RemoveVoteAsync(int solutionId, int userId)
    {
        var vote = await _context.Votes
            .FirstOrDefaultAsync(v => v.SolutionId == solutionId && v.UserId == userId);

        if (vote == null) return null;

        _context.Votes.Remove(vote);
        await _context.SaveChangesAsync();
        await RecalculateScoreAsync(solutionId);
        return await GetSolutionDtoAsync(solutionId, userId);
    }

    private async Task RecalculateScoreAsync(int solutionId)
    {
        var solution = await _context.Solutions
            .Include(s => s.Votes)
            .FirstAsync(s => s.Id == solutionId);

        solution.Score = solution.Votes.Sum(v => v.Value);
        await _context.SaveChangesAsync();
    }

    private async Task<SolutionDto?> GetSolutionDtoAsync(int solutionId, int userId)
    {
        var solution = await _context.Solutions
            .Include(s => s.Author)
            .Include(s => s.Votes)
            .FirstOrDefaultAsync(s => s.Id == solutionId);

        if (solution == null) return null;

        return new SolutionDto
        {
            Id = solution.Id,
            ProblemId = solution.ProblemId,
            AuthorId = solution.AuthorId,
            AuthorUsername = solution.Author.Username,
            Content = solution.Content,
            Score = solution.Score,
            Upvotes = solution.Votes.Count(v => v.Value == 1),
            Downvotes = solution.Votes.Count(v => v.Value == -1),
            UserVote = solution.Votes.FirstOrDefault(v => v.UserId == userId)?.Value,
            CreatedAt = solution.CreatedAt,
            UpdatedAt = solution.UpdatedAt
        };
    }
}
