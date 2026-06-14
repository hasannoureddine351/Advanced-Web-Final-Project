using UniSolve.Api.DTOs;

namespace UniSolve.Api.Services.Interfaces;

public interface ISubjectService
{
    Task<List<SubjectDto>> GetAllAsync();
    Task<SubjectDto?> GetByIdAsync(int id);
    Task<SubjectDto> CreateAsync(CreateSubjectRequest request);
    Task<SubjectDto?> UpdateAsync(int id, UpdateSubjectRequest request);
    Task<bool> DeleteAsync(int id);
}
