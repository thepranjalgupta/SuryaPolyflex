using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Core;

public enum RuleScope
{
    All = 0,
    Owner = 1,
    Department = 2,
    OwnerOrDepartment = 3
}

public class Rule : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int PermissionId { get; set; }
    public RuleScope Scope { get; set; } = RuleScope.All;
    public bool IsActive { get; set; } = true;

    // Navigation
    public Permission Permission { get; set; } = default!;
}