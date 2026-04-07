using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Core;

public class ApprovalWorkflow : BaseEntity
{
    public string Module { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; } = true;

    public ICollection<ApprovalStep> Steps { get; set; } = new List<ApprovalStep>();
}