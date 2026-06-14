using Microsoft.EntityFrameworkCore;
using UniSolve.Api.Data;
using UniSolve.Api.DTOs;
using UniSolve.Api.Models.Entities;
using UniSolve.Api.Services.Interfaces;

namespace UniSolve.Api.Services.Implementations;

public class SubjectService : ISubjectService
{
    private readonly ApplicationDbContext _context;

    public SubjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubjectDto>> GetAllAsync()
    {
        return await _context.Subjects
            .OrderBy(s => s.Name)
            .Select(s => MapSubject(s))
            .ToListAsync();
    }

    public async Task<SubjectDto?> GetByIdAsync(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        return subject == null ? null : MapSubject(subject);
    }

    public async Task<SubjectDto> CreateAsync(CreateSubjectRequest request)
    {
        var subject = new Subject
        {
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant()
        };

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();
        return MapSubject(subject);
    }

    public async Task<SubjectDto?> UpdateAsync(int id, UpdateSubjectRequest request)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null) return null;

        subject.Name = request.Name.Trim();
        subject.Code = request.Code.Trim().ToUpperInvariant();
        await _context.SaveChangesAsync();
        return MapSubject(subject);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null) return false;

        var hasProblems = await _context.Problems.AnyAsync(p => p.SubjectId == id);
        if (hasProblems) return false;

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        return true;
    }

    private static SubjectDto MapSubject(Subject subject) => new()
    {
        Id = subject.Id,
        Name = subject.Name,
        Code = subject.Code
    };
}
