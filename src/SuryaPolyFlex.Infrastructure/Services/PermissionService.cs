using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Infrastructure.Data;
using SuryaPolyFlex.Application.Common;


namespace SuryaPolyFlex.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PermissionService(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || !user.IsActive) return false;

        // Admin role always has full access
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Admin")) return true;

        // Check role-based permissions
        var roleIds = await _context.Roles
            .Where(r => roles.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync();

        return await _context.RoleModulePermissions
            .AnyAsync(p =>
                roleIds.Contains(p.RoleId) &&
                p.Action == permission &&
                p.IsAllowed);
    }

    public async Task<List<string>> GetUserPermissionsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return new List<string>();

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Admin"))
        {
            // Admin gets all permissions
            return GetAllPermissions();
        }

        var roleIds = await _context.Roles
            .Where(r => roles.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync();

        return await _context.RoleModulePermissions
            .Where(p => roleIds.Contains(p.RoleId) && p.IsAllowed)
            .Select(p => p.Action)
            .Distinct()
            .ToListAsync();
    }

    public async Task GrantPermissionAsync(string roleId, string permission)
    {
        var existing = await _context.RoleModulePermissions
            .FirstOrDefaultAsync(p => p.RoleId == roleId && p.Action == permission);

        if (existing == null)
        {
            _context.RoleModulePermissions.Add(new RoleModulePermission
            {
                RoleId = roleId,
                Action = permission,
                ModuleCode = permission.Split('_')[0],
                IsAllowed = true
            });
        }
        else
        {
            existing.IsAllowed = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RevokePermissionAsync(string roleId, string permission)
    {
        var existing = await _context.RoleModulePermissions
            .FirstOrDefaultAsync(p => p.RoleId == roleId && p.Action == permission);

        if (existing != null)
        {
            existing.IsAllowed = false;
            await _context.SaveChangesAsync();
        }
    }

    private static List<string> GetAllPermissions()
    {
        return typeof(Permissions)
            .GetNestedTypes()
            .SelectMany(t => t.GetFields())
            .Select(f => f.GetValue(null)?.ToString() ?? "")
            .Where(v => !string.IsNullOrEmpty(v))
            .ToList();
    }
}