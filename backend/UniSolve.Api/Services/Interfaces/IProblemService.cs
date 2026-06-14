using UniSolve.Api.DTOs;

namespace UniSolve.Api.Services.Interfaces;

public interface IProblemService
{
    Task<PagedResult<ProblemListItemDto>> GetAllAsync(int? subjectId, string? search, int page, int pageSize);
    Task<ProblemDetailDto?> GetByIdAsync(int id, int? currentUserId);
    Task<List<ProblemListItemDto>> GetMineAsync(int userId);
    Task<ProblemDetailDto?> CreateAsync(CreateProblemRequest request, int userId);
    Task<ProblemDetailDto?> UpdateAsync(int id, UpdateProblemRequest request, int userId, bool isAdmin);
    Task<bool> DeleteAsync(int id, int userId, bool isAdmin);
}
