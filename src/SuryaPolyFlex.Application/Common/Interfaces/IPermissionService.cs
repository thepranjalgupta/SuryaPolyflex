using SuryaPolyFlex.Domain.Entities.Core;

namespace SuryaPolyFlex.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string userId, string permission, object? resourceContext = null);
    Task<List<string>> GetUserPermissionsAsync(string userId);
    Task GrantPermissionAsync(string roleId, string permission);
    Task RevokePermissionAsync(string roleId, string permission);

    // Admin management
    Task<List<Permission>> GetAllPermissionsAsync();
    Task<Permission?> GetPermissionByIdAsync(int permissionId);
    Task<Permission> CreatePermissionAsync(Permission permission);
    Task<bool> UpdatePermissionAsync(Permission permission);
    Task<bool> DeletePermissionAsync(int permissionId);

    Task<List<RolePermission>> GetRolePermissionsAsync(string roleId);
    Task<List<Rule>> GetRulesByPermissionIdAsync(int permissionId);
    Task<Rule> CreateRuleAsync(Rule rule);
    Task<bool> UpdateRuleAsync(Rule rule);
    Task<bool> DeleteRuleAsync(int ruleId);

    // User Permission Overrides
    Task<UserPermission> GrantUserPermissionAsync(string userId, int permissionId, string? createdBy = null, DateTime? expiryDate = null);
    Task<UserPermission> DenyUserPermissionAsync(string userId, int permissionId, string? createdBy = null, DateTime? expiryDate = null);
    Task<bool> RevokeUserPermissionAsync(string userId, int permissionId);
    Task<List<UserPermission>> GetUserPermissionOverridesAsync(string userId);
}