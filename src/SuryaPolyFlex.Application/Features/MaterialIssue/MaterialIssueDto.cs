using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.MaterialIssue;

public class MaterialIssueDto
{
    public int Id { get; set; }
    public string IssueNo { get; set; } = default!;
    public DateTime IssueDate { get; set; }
    public string WarehouseName { get; set; } = default!;
    public string? DepartmentName { get; set; }
    public string? Remarks { get; set; }
    public List<MaterialIssueItemDto> Items { get; set; } = new();
}

public class MaterialIssueItemDto
{
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = default!;
    public string ItemName { get; set; } = default!;
    public string? UoMCode { get; set; }
    public decimal RequestedQty { get; set; }
    public decimal IssuedQty { get; set; }
}

public class CreateMaterialIssueDto
{
    [Required] public int FromWarehouseId { get; set; }
    public int? ToDepartmentId { get; set; }
    public int? WorkOrderId { get; set; }
    public string? Remarks { get; set; }
    [Required] public List<CreateMaterialIssueItemDto> Items { get; set; } = new();
}

public class CreateMaterialIssueItemDto
{
    [Required] public int ItemId { get; set; }
    [Required, Range(0.001, double.MaxValue)] public decimal RequestedQty { get; set; }
    [Required, Range(0.001, double.MaxValue)] public decimal IssuedQty { get; set; }
    public int? UoMId { get; set; }
}