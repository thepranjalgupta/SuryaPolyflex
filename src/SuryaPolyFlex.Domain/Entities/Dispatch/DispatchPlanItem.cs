using SuryaPolyFlex.Domain.Entities.Sales;

namespace SuryaPolyFlex.Domain.Entities.Dispatch;

public class DispatchPlanItem
{
    public int Id { get; set; }
    public int DispatchPlanId { get; set; }
    public int SOId { get; set; }
    public decimal DispatchQty { get; set; }

    public DispatchPlan DispatchPlan { get; set; } = default!;
    public SalesOrder SalesOrder { get; set; } = default!;
}