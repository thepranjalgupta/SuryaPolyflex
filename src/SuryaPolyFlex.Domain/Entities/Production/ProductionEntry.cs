using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Production;

public class ProductionEntry : BaseEntity
{
    public int WorkOrderId { get; set; }
    public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    public decimal ProducedQty { get; set; }
    public decimal WastageQty { get; set; } = 0;
    public string? WastageReason { get; set; }
    public int MachineDowntimeMin { get; set; } = 0;
    public string? DowntimeReason { get; set; }
    public string OperatorId { get; set; } = default!;

    public WorkOrder WorkOrder { get; set; } = default!;
}