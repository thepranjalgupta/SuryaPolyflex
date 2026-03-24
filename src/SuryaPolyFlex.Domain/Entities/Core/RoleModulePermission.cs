namespace SuryaPolyFlex.Domain.Entities.Core;

public class RoleModulePermission
{
    public int Id { get; set; }
    public string RoleId { get; set; } = default!;
    public string ModuleCode { get; set; } = default!;   // PROCUREMENT, SALES, PRODUCTION
    public string Action { get; set; } = default!;        // VIEW, CREATE, EDIT, DELETE, APPROVE, EXPORT
    public bool IsAllowed { get; set; } = false;

    public ApplicationRole Role { get; set; } = default!;
}