using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Enums;

namespace SuryaPolyFlex.Domain.Entities.Production;

public class WorkOrder : BaseEntity
{
    public string WONumber { get; set; } = default!;
    public int JobCardId { get; set; }
    public int? MachineId { get; set; }
    public string? OperatorId { get; set; }
    public string Shift { get; set; } = default!;
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;

    public JobCard JobCard { get; set; } = default!;
    public Machine? Machine { get; set; }
    public ICollection<ProductionEntry> ProductionEntries { get; set; } = new List<ProductionEntry>();
}