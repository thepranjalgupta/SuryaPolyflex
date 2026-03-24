using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Features.Users;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AppDbContext _context;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context     = context;
    }

    public async Task<List<UserDto>> GetAllAsync(string? search = null)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u =>
                u.FullName.Contains(search) ||
                u.Email!.Contains(search));

        var users = await query
            .OrderBy(u => u.FullName)
            .ToListAsync();

        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserDto
            {
                Id          = user.Id,
                FullName    = user.FullName,
                Email       = user.Email!,
                Phone       = user.PhoneNumber,
                IsActive    = user.IsActive,
                CreatedAt   = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Roles       = roles.ToList()
            });
        }

        return result;
    }

    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id          = user.Id,
            FullName    = user.FullName,
            Email       = user.Email!,
            Phone       = user.PhoneNumber,
            IsActive    = user.IsActive,
            CreatedAt   = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles       = roles.ToList()
        };
    }

    public async Task<(bool Success, string Message)> CreateAsync(
        CreateUserDto dto, string createdBy)
    {
        var exists = await _userManager.FindByEmailAsync(dto.Email);
        if (exists != null)
            return (false, $"Email '{dto.Email}' is already registered.");

        if (!await _roleManager.RoleExistsAsync(dto.RoleName))
            return (false, $"Role '{dto.RoleName}' does not exist.");

        var user = new ApplicationUser
        {
            UserName      = dto.Email,
            Email         = dto.Email,
            FullName      = dto.FullName.Trim(),
            PhoneNumber   = dto.Phone?.Trim(),
            EmployeeId    = dto.EmployeeId,
            DepartmentId  = dto.DepartmentId,
            IsActive      = true,
            EmailConfirmed = true,
            CreatedAt     = DateTime.UtcNow,
            CreatedBy     = createdBy
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, errors);
        }

        await _userManager.AddToRoleAsync(user, dto.RoleName);
        return (true, "User created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(EditUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.Id);
        if (user == null)
            return (false, "User not found.");

        if (!await _roleManager.RoleExistsAsync(dto.RoleName))
            return (false, $"Role '{dto.RoleName}' does not exist.");

        // Update fields
        user.FullName     = dto.FullName.Trim();
        user.PhoneNumber  = dto.Phone?.Trim();
        user.EmployeeId   = dto.EmployeeId;
        user.DepartmentId = dto.DepartmentId;
        user.IsActive     = dto.IsActive;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return (false, errors);
        }

        // Update role
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, dto.RoleName);

        return (true, "User updated successfully.");
    }

    public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null)
            return (false, "User not found.");

        var token  = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, errors);
        }

        return (true, "Password reset successfully.");
    }

    public async Task<(bool Success, string Message)> ToggleActiveAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return (false, "User not found.");

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);

        return (true, user.IsActive ? "User activated." : "User deactivated.");
    }

    public async Task<List<string>> GetAllRolesAsync()
    {
        return await _roleManager.Roles
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .Select(r => r.Name!)
            .ToListAsync();
    }
}