using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Core;

public class RolePermission : BaseEntity
{
    public string RoleId { get; set; } = default!;
    public int PermissionId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ApplicationRole Role { get; set; } = default!;
    public Permission Permission { get; set; } = default!;
}