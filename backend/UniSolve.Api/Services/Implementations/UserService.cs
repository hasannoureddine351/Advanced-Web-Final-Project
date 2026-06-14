using Microsoft.EntityFrameworkCore;
using UniSolve.Api.Data;
using UniSolve.Api.DTOs;
using UniSolve.Api.Models.Enums;
using UniSolve.Api.Services.Interfaces;

namespace UniSolve.Api.Services.Implementations;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserAdminDto>> GetAllAsync()
    {
        return await _context.Users
            .OrderBy(u => u.Username)
            .Select(u => new UserAdminDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                Role = u.Role.ToString(),
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<UserAdminDto?> UpdateRoleAsync(int userId, string role, int adminUserId)
    {
        if (userId == adminUserId)
            return null;

        if (!Enum.TryParse<UserRole>(role, ignoreCase: true, out var newRole))
            return null;

        if (newRole is not (UserRole.Student or UserRole.Verified or UserRole.Admin))
            return null;

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        user.Role = newRole;
        await _context.SaveChangesAsync();

        return new UserAdminDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}
