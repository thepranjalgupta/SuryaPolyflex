using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.Indents;

public class IndentDto
{
    public int Id { get; set; }
    public string IndentNumber { get; set; } = default!;
    public DateTime IndentDate { get; set; }
    public string DepartmentName { get; set; } = default!;
    public string RequestedByName { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? Remarks { get; set; }
    public List<IndentItemDto> Items { get; set; } = new();
}

public class IndentItemDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = default!;
    public string ItemName { get; set; } = default!;
    public string UoMCode { get; set; } = default!;
    public decimal RequestedQty { get; set; }
    public decimal ApprovedQty { get; set; }
    public string? Remarks { get; set; }
}

public class CreateIndentDto
{
    [Required] public int DepartmentId { get; set; }
    public string? Remarks { get; set; }

    [Required, MinLength(1, ErrorMessage = "Add at least one item.")]
    public List<CreateIndentItemDto> Items { get; set; } = new();
}

public class CreateIndentItemDto
{
    [Required] public int ItemId { get; set; }
    [Required, Range(0.001, double.MaxValue, ErrorMessage = "Qty must be greater than 0")]
    public decimal RequestedQty { get; set; }
    public string? Remarks { get; set; }
}

public class ApproveIndentDto
{
    public int IndentId { get; set; }
    public string Action { get; set; } = default!; // Approve or Reject
    public string? Remarks { get; set; }
    public List<ApproveIndentItemDto> Items { get; set; } = new();
}

public class ApproveIndentItemDto
{
    public int IndentItemId { get; set; }
    public decimal ApprovedQty { get; set; }
}