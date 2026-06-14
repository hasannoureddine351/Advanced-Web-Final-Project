using UniSolve.Api.DTOs;

namespace UniSolve.Api.Services.Interfaces;

public interface IVoteService
{
    Task<SolutionDto?> VoteAsync(int solutionId, int value, int userId);
    Task<SolutionDto?> RemoveVoteAsync(int solutionId, int userId);
}
