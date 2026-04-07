using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Core;

public class ApprovalStep : BaseEntity
{
    public int WorkflowId { get; set; }
    public int StepOrder { get; set; }
    public string RoleName { get; set; } = default!;
    public bool IsFinal { get; set; } = false;
    public bool IsDepartmentScoped { get; set; } = true;

    public ApprovalWorkflow Workflow { get; set; } = default!;
}