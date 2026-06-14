using UniSolve.Api.DTOs;

namespace UniSolve.Api.Services.Interfaces;

public interface IUserService
{
    Task<List<UserAdminDto>> GetAllAsync();
    Task<UserAdminDto?> UpdateRoleAsync(int userId, string role, int adminUserId);
}
