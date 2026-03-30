using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Enums;

namespace SuryaPolyFlex.Domain.Entities.Production;

public class JobCard : BaseEntity
{
    public string JobCardNo { get; set; } = default!;
    public int? CustomerJobId { get; set; }
    public int? SOId { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public int? MachineId { get; set; }
    public string? AssignedOperatorId { get; set; }
    public string? Shift { get; set; }
    public decimal TargetQty { get; set; }
    public int? UoMId { get; set; }
    public JobCardStatus Status { get; set; } = JobCardStatus.Created;
    public string? Remarks { get; set; }

    public Machine? Machine { get; set; }
    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    public ICollection<BOM> BOMs { get; set; } = new List<BOM>();
}