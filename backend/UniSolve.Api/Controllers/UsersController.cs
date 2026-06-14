using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSolve.Api.DTOs;
using UniSolve.Api.Services.Interfaces;

namespace UniSolve.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserAdminDto>>> GetAll()
    {
        return Ok(await _userService.GetAllAsync());
    }

    [HttpPut("{id}/role")]
    public async Task<ActionResult<UserAdminDto>> UpdateRole(int id, UpdateUserRoleRequest request)
    {
        var result = await _userService.UpdateRoleAsync(id, request.Role, User.GetUserId());
        if (result == null)
            return BadRequest(new { message = "Invalid role, user not found, or cannot change your own role." });

        return Ok(result);
    }
}
