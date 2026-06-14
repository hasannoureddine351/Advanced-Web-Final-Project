using Microsoft.EntityFrameworkCore;
using UniSolve.Api.Data;
using UniSolve.Api.DTOs;
using UniSolve.Api.Models.Entities;
using UniSolve.Api.Services.Interfaces;

namespace UniSolve.Api.Services.Implementations;

public class ProblemService : IProblemService
{
    private readonly ApplicationDbContext _context;

    public ProblemService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProblemListItemDto>> GetAllAsync(int? subjectId, string? search, int page, int pageSize)
    {
        var query = _context.Problems
            .Include(p => p.Subject)
            .Include(p => p.Author)
            .AsQueryable();

        if (subjectId.HasValue)
            query = query.Where(p => p.SubjectId == subjectId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(term) ||
                p.Description.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProblemListItemDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                BookReference = p.BookReference,
                ProblemNumber = p.ProblemNumber,
                SubjectId = p.SubjectId,
                SubjectName = p.Subject.Name,
                AuthorId = p.AuthorId,
                AuthorUsername = p.Author.Username,
                SolutionCount = p.Solutions.Count,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<ProblemListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProblemDetailDto?> GetByIdAsync(int id, int? currentUserId)
    {
        var problem = await _context.Problems
            .Include(p => p.Subject)
            .Include(p => p.Author)
            .Include(p => p.Solutions)
                .ThenInclude(s => s.Author)
            .Include(p => p.Solutions)
                .ThenInclude(s => s.Votes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (problem == null) return null;

        return new ProblemDetailDto
        {
            Id = problem.Id,
            Title = problem.Title,
            Description = problem.Description,
            BookReference = problem.BookReference,
            ProblemNumber = problem.ProblemNumber,
            SubjectId = problem.SubjectId,
            SubjectName = problem.Subject.Name,
            AuthorId = problem.AuthorId,
            AuthorUsername = problem.Author.Username,
            CreatedAt = problem.CreatedAt,
            UpdatedAt = problem.UpdatedAt,
            Solutions = problem.Solutions
                .OrderByDescending(s => s.Score)
                .ThenByDescending(s => s.CreatedAt)
                .Select(s => MapSolution(s, currentUserId))
                .ToList()
        };
    }

    public async Task<List<ProblemListItemDto>> GetMineAsync(int userId)
    {
        return await _context.Problems
            .Include(p => p.Subject)
            .Include(p => p.Author)
            .Where(p => p.AuthorId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProblemListItemDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                BookReference = p.BookReference,
                ProblemNumber = p.ProblemNumber,
                SubjectId = p.SubjectId,
                SubjectName = p.Subject.Name,
                AuthorId = p.AuthorId,
                AuthorUsername = p.Author.Username,
                SolutionCount = p.Solutions.Count,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ProblemDetailDto?> CreateAsync(CreateProblemRequest request, int userId)
    {
        if (!await _context.Subjects.AnyAsync(s => s.Id == request.SubjectId))
            return null;

        var problem = new Problem
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            BookReference = request.BookReference?.Trim(),
            ProblemNumber = request.ProblemNumber?.Trim(),
            SubjectId = request.SubjectId,
            AuthorId = userId
        };

        _context.Problems.Add(problem);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(problem.Id, userId);
    }

    public async Task<ProblemDetailDto?> UpdateAsync(int id, UpdateProblemRequest request, int userId, bool isAdmin)
    {
        var problem = await _context.Problems.FindAsync(id);
        if (problem == null) return null;
        if (problem.AuthorId != userId && !isAdmin) return null;

        if (!await _context.Subjects.AnyAsync(s => s.Id == request.SubjectId))
            return null;

        problem.Title = request.Title.Trim();
        problem.Description = request.Description.Trim();
        problem.BookReference = request.BookReference?.Trim();
        problem.ProblemNumber = request.ProblemNumber?.Trim();
        problem.SubjectId = request.SubjectId;
        problem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id, userId);
    }

    public async Task<bool> DeleteAsync(int id, int userId, bool isAdmin)
    {
        var problem = await _context.Problems.FindAsync(id);
        if (problem == null) return false;
        if (problem.AuthorId != userId && !isAdmin) return false;

        _context.Problems.Remove(problem);
        await _context.SaveChangesAsync();
        return true;
    }

    private static SolutionDto MapSolution(Solution solution, int? currentUserId)
    {
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
