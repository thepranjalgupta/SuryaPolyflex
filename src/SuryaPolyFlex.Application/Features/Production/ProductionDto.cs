using System.ComponentModel.DataAnnotations;
using SuryaPolyFlex.Application.Features.SalesOrders;

namespace SuryaPolyFlex.Application.Features.Production;

public class MachineDto
{
    public int Id { get; set; }
    public string MachineCode { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Type { get; set; }
    public bool IsActive { get; set; }
}

public class CreateMachineDto
{
    [Required] public string MachineCode { get; set; } = default!;
    [Required] public string Name { get; set; } = default!;
    public string? Type { get; set; }
}

public class JobCardDto
{
    public int Id { get; set; }
    public string JobCardNo { get; set; } = default!;
    public int? SOId { get; set; }
    public string? SONumber { get; set; }
    public DateTime? SODate { get; set; }
    public DateTime? SORequiredBy { get; set; }
    public string? SOCustomerName { get; set; }
    public string? SOStatus { get; set; }
    public int? CustomerJobId { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public string? MachineName { get; set; }
    public string? Shift { get; set; }
    public decimal TargetQty { get; set; }
    public string Status { get; set; } = default!;
    public string? Remarks { get; set; }
    public List<SOItemDto> SOItems { get; set; } = new();
    public List<CustomerJobDto> CustomerJobs { get; set; } = new();
    public List<WorkOrderDto> WorkOrders { get; set; } = new();
    public List<BOMDto> BOMs { get; set; } = new();
}

public class CreateJobCardDto
{
    public int? SOId { get; set; }
    public int? CustomerJobId { get; set; }
    [Required] public DateTime PlannedStartDate { get; set; }
    [Required] public DateTime PlannedEndDate { get; set; }
    public int? MachineId { get; set; }
    public string? AssignedOperatorId { get; set; }
    public string? Shift { get; set; }
    [Required, Range(0.001, double.MaxValue)] public decimal TargetQty { get; set; }
    public int? UoMId { get; set; }
    public string? Remarks { get; set; }
}

public class BOMDto
{
    public int Id { get; set; }
    public string BOMNo { get; set; } = default!;
    public int JobCardId { get; set; }
    public string Status { get; set; } = default!;
    public string? Remarks { get; set; }
    public List<BOMItemDto> Items { get; set; } = new();
}

public class BOMItemDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = default!;
    public string ItemName { get; set; } = default!;
    public string? UoMCode { get; set; }
    public decimal RequiredQty { get; set; }
    public decimal IssuedQty { get; set; }
}

public class CreateBOMDto
{
    [Required] public int JobCardId { get; set; }
    public string? Remarks { get; set; }
    [Required] public List<CreateBOMItemDto> Items { get; set; } = new();
}

public class CreateBOMItemDto
{
    [Required] public int ItemId { get; set; }
    [Required, Range(0.001, double.MaxValue)] public decimal RequiredQty { get; set; }
    public int? UoMId { get; set; }
}

public class WorkOrderDto
{
    public int Id { get; set; }
    public string WONumber { get; set; } = default!;
    public int JobCardId { get; set; }
    public string? JobCardNo { get; set; }
    public string? SONumber { get; set; }
    public DateTime? SODate { get; set; }
    public DateTime? SORequiredBy { get; set; }
    public string? SOCustomerName { get; set; }
    public string? SOStatus { get; set; }
    public string? MachineName { get; set; }
    public string Shift { get; set; } = default!;
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string Status { get; set; } = default!;
    public decimal TotalProduced { get; set; }
    public decimal TotalWastage { get; set; }
    public List<ProductionEntryDto> Entries { get; set; } = new();
}

public class CreateWorkOrderDto
{
    [Required] public int JobCardId { get; set; }
    public int? MachineId { get; set; }
    public string? OperatorId { get; set; }
    [Required] public string Shift { get; set; } = default!;
}

public class ProductionEntryDto
{
    public int Id { get; set; }
    public DateTime EntryDate { get; set; }
    public decimal ProducedQty { get; set; }
    public decimal WastageQty { get; set; }
    public string? WastageReason { get; set; }
    public int MachineDowntimeMin { get; set; }
    public string? DowntimeReason { get; set; }
}

public class CreateProductionEntryDto
{
    [Required] public int WorkOrderId { get; set; }
    [Required, Range(0.001, double.MaxValue)] public decimal ProducedQty { get; set; }
    public decimal WastageQty { get; set; } = 0;
    public string? WastageReason { get; set; }
    public int MachineDowntimeMin { get; set; } = 0;
    public string? DowntimeReason { get; set; }
    public string? OperatorId { get; set; }
}