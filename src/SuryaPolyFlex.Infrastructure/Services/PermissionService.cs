using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Infrastructure.Data;
using SuryaPolyFlex.Application.Common;


namespace SuryaPolyFlex.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRuleEvaluationService _ruleEvaluationService;
    private readonly IMemoryCache _cache;

    public PermissionService(AppDbContext context, UserManager<ApplicationUser> userManager, IRuleEvaluationService ruleEvaluationService, IMemoryCache cache)
    {
        _context = context;
        _userManager = userManager;
        _ruleEvaluationService = ruleEvaluationService;
        _cache = cache;
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission, object? resourceContext = null)
    {
        var cacheKey = $"permission_{userId}_{permission}_{resourceContext?.GetHashCode() ?? 0}";

        if (_cache.TryGetValue(cacheKey, out bool cachedResult))
        {
            return cachedResult;
        }

        var result = await HasPermissionInternalAsync(userId, permission, resourceContext);

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }

    private async Task<bool> HasPermissionInternalAsync(string userId, string permission, object? resourceContext = null)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || !user.IsActive) return false;

        // ✅ Step 1: Check user-level permission overrides (highest priority)
        var permissionId = await _context.Permissions
            .Where(p => p.Name == permission && !p.IsDeleted)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        if (permissionId > 0)
        {
            var userOverride = await _context.UserPermissions
                .Where(up =>
                    up.UserId == userId &&
                    up.PermissionId == permissionId &&
                    up.IsActive &&
                    (up.ExpiryDate == null || up.ExpiryDate > DateTime.UtcNow))
                .FirstOrDefaultAsync();

            if (userOverride != null)
            {
                // Return based on override: true = allow, false = deny
                return userOverride.IsAllowed;
            }
        }

        // ✅ Step 2: Check Admin role (legacy bypass)
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Admin")) return true;

        // ✅ Step 3: Check role-based permissions
        var roleIds = await _context.Roles
            .Where(r => roles.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync();

        var hasRolePermission = await _context.RolePermissions
            .Include(rp => rp.Permission)
            .AnyAsync(rp =>
                roleIds.Contains(rp.RoleId) &&
                rp.Permission.Name == permission &&
                rp.IsActive);

        if (!hasRolePermission) return false;

        // ✅ Step 4: If resource context provided, evaluate scope rules
        if (resourceContext != null)
        {
            object? wrappedContext = resourceContext;
            if (resourceContext is not Dictionary<string, object>)
            {
                // Accept anonymous type -> convert to dictionary for rule service
                wrappedContext = ToDictionary(resourceContext);
            }

            return await _ruleEvaluationService.EvaluateRulesAsync(permission, userId, wrappedContext!);
        }

        return true;
    }

    private Dictionary<string, object> ToDictionary(object source)
    {
        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var prop in source.GetType().GetProperties())
        {
            var value = prop.GetValue(source);
            if (value != null)
                dict[prop.Name] = value;
        }

        return dict;
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

        return await _context.RolePermissions
            .Include(rp => rp.Permission)
            .Where(rp => roleIds.Contains(rp.RoleId) && rp.IsActive)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync();
    }

    public async Task GrantPermissionAsync(string roleId, string permissionName)
    {
        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == permissionName);

        if (permission == null) return; // Or throw exception

        var existing = await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

        if (existing == null)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permission.Id,
                IsActive = true
            });
        }
        else
        {
            existing.IsActive = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RevokePermissionAsync(string roleId, string permissionName)
    {
        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == permissionName);

        if (permission == null) return;

        var existing = await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

        if (existing != null)
        {
            existing.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Permission>> GetAllPermissionsAsync()
    {
        return await _context.Permissions
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<Permission?> GetPermissionByIdAsync(int permissionId)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == permissionId && !p.IsDeleted);
    }

    public async Task<Permission> CreatePermissionAsync(Permission permission)
    {
        permission.CreatedAt = DateTime.UtcNow;
        permission.IsDeleted = false;

        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        return permission;
    }

    public async Task<bool> UpdatePermissionAsync(Permission permission)
    {
        var existing = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == permission.Id && !p.IsDeleted);

        if (existing == null) return false;

        existing.Name = permission.Name;
        existing.Module = permission.Module;
        existing.Action = permission.Action;
        existing.Description = permission.Description;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = permission.UpdatedBy;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePermissionAsync(int permissionId)
    {
        var existing = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == permissionId && !p.IsDeleted);

        if (existing == null) return false;

        existing.IsDeleted = true;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<RolePermission>> GetRolePermissionsAsync(string roleId)
    {
        return await _context.RolePermissions
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId && !rp.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<Rule>> GetRulesByPermissionIdAsync(int permissionId)
    {
        return await _context.Rules
            .Where(r => r.PermissionId == permissionId && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<Rule> CreateRuleAsync(Rule rule)
    {
        // Ensure permission exists before creating a rule to avoid FK violation.
        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == rule.PermissionId && !p.IsDeleted);

        if (permission == null)
            throw new InvalidOperationException($"Cannot create rule. Permission with Id {rule.PermissionId} does not exist.");

        rule.CreatedAt = DateTime.UtcNow;
        rule.IsDeleted = false;

        _context.Rules.Add(rule);
        await _context.SaveChangesAsync();

        return rule;
    }

    public async Task<bool> UpdateRuleAsync(Rule rule)
    {
        var existing = await _context.Rules
            .FirstOrDefaultAsync(r => r.Id == rule.Id && !r.IsDeleted);

        if (existing == null) return false;

        existing.Name = rule.Name;
        existing.Description = rule.Description;
        existing.Scope = rule.Scope;
        existing.IsActive = rule.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = rule.UpdatedBy;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRuleAsync(int ruleId)
    {
        var existing = await _context.Rules
            .FirstOrDefaultAsync(r => r.Id == ruleId && !r.IsDeleted);

        if (existing == null) return false;

        existing.IsDeleted = true;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // ✅ UserPermission Override Management

    /// <summary>
    /// Grant a specific permission to a user (override), bypassing role checks.
    /// Only available for Admin users.
    /// </summary>
    public async Task<UserPermission> GrantUserPermissionAsync(
        string userId,
        int permissionId,
        string? createdBy = null,
        DateTime? expiryDate = null)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException($"User with ID {userId} not found.");

        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == permissionId && !p.IsDeleted);
        if (permission == null)
            throw new InvalidOperationException($"Permission with ID {permissionId} not found.");

        // Check if override already exists
        var existing = await _context.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);

        if (existing != null)
        {
            existing.IsAllowed = true;
            existing.IsActive = true;
            existing.ExpiryDate = expiryDate;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = createdBy;
        }
        else
        {
            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                IsAllowed = true,
                IsActive = true,
                ExpiryDate = expiryDate,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserPermissions.Add(userPermission);
            existing = userPermission;
        }

        await _context.SaveChangesAsync();
        InvalidateCacheForUser(userId);

        return existing;
    }

    /// <summary>
    /// Deny a specific permission to a user (explicit denial override).
    /// Only available for Admin users.
    /// </summary>
    public async Task<UserPermission> DenyUserPermissionAsync(
        string userId,
        int permissionId,
        string? createdBy = null,
        DateTime? expiryDate = null)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException($"User with ID {userId} not found.");

        var permission = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == permissionId && !p.IsDeleted);
        if (permission == null)
            throw new InvalidOperationException($"Permission with ID {permissionId} not found.");

        var existing = await _context.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);

        if (existing != null)
        {
            existing.IsAllowed = false;
            existing.IsActive = true;
            existing.ExpiryDate = expiryDate;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = createdBy;
        }
        else
        {
            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                IsAllowed = false,
                IsActive = true,
                ExpiryDate = expiryDate,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserPermissions.Add(userPermission);
            existing = userPermission;
        }

        await _context.SaveChangesAsync();
        InvalidateCacheForUser(userId);

        return existing;
    }

    /// <summary>
    /// Revoke a user-level permission override.
    /// </summary>
    public async Task<bool> RevokeUserPermissionAsync(string userId, int permissionId)
    {
        var existing = await _context.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);

        if (existing == null) return false;

        _context.UserPermissions.Remove(existing);
        await _context.SaveChangesAsync();
        InvalidateCacheForUser(userId);

        return true;
    }

    /// <summary>
    /// Get all active permission overrides for a user.
    /// </summary>
    public async Task<List<UserPermission>> GetUserPermissionOverridesAsync(string userId)
    {
        return await _context.UserPermissions
            .AsNoTracking()
            .Include(up => up.Permission)
            .Where(up =>
                up.UserId == userId &&
                up.IsActive &&
                (up.ExpiryDate == null || up.ExpiryDate > DateTime.UtcNow))
            .ToListAsync();
    }

    /// <summary>
    /// Invalidate cache entries for a user when permissions change.
    /// </summary>
    private void InvalidateCacheForUser(string userId)
    {
        // In a production system, you'd want to invalidate all cache keys matching this user
        // For now, this is a placeholder. Consider using IDistributedCache for better cache invalidation strategy.
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