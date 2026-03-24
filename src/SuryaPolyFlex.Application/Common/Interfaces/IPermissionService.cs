namespace SuryaPolyFlex.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string userId, string permission);
    Task<List<string>> GetUserPermissionsAsync(string userId);
    Task GrantPermissionAsync(string roleId, string permission);
    Task RevokePermissionAsync(string roleId, string permission);
}