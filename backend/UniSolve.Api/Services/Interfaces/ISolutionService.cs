using UniSolve.Api.DTOs;

namespace UniSolve.Api.Services.Interfaces;

public interface ISolutionService
{
    Task<SolutionDto?> CreateAsync(int problemId, CreateSolutionRequest request, int userId);
    Task<SolutionDto?> UpdateAsync(int id, UpdateSolutionRequest request, int userId, bool isModerator);
    Task<bool> DeleteAsync(int id, int userId, bool isModerator);
}
