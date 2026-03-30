using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Dispatch;

public class DispatchPlan : BaseEntity
{
    public string PlanNo { get; set; } = default!;
    public DateTime PlannedDate { get; set; }
    public string Status { get; set; } = "Draft";
    public string? Remarks { get; set; }

    public ICollection<DispatchPlanItem> Items { get; set; } = new List<DispatchPlanItem>();
}