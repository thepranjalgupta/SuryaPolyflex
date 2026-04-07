using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Core;

public class Permission : BaseEntity
{
    public string Name { get; set; } = default!; // e.g., "Users.View"
    public string Module { get; set; } = default!; // e.g., "Users"
    public string Action { get; set; } = default!; // e.g., "View"
    public string Description { get; set; } = default!;

    // Navigation
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<Rule> Rules { get; set; } = new List<Rule>();
}