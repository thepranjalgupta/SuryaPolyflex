using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.SalesOrders;

public class SalesReturnDto
{
    public int Id { get; set; }
    public string ReturnNo { get; set; } = default!;
    public DateTime ReturnDate { get; set; }
    public string SONumber { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
    public string Reason { get; set; } = default!;
    public decimal TotalReturnValue { get; set; }
    public string Status { get; set; } = default!;
    public List<SalesReturnItemDto> Items { get; set; } = new();
}

public class SalesReturnItemDto
{
    public string Description { get; set; } = default!;
    public decimal ReturnQty { get; set; }
    public decimal UnitRate { get; set; }
    public decimal LineTotal => ReturnQty * UnitRate;
    public string? ReturnReason { get; set; }
}

public class CreateSalesReturnDto
{
    [Required] public int SOId { get; set; }
    [Required] public string Reason { get; set; } = default!;
    public string? Remarks { get; set; }
    [Required] public List<CreateSalesReturnItemDto> Items { get; set; } = new();
}

public class CreateSalesReturnItemDto
{
    public int? ItemId { get; set; }
    [Required] public string Description { get; set; } = default!;
    [Required, Range(0.001, double.MaxValue)] public decimal ReturnQty { get; set; }
    public int? UoMId { get; set; }
    public decimal UnitRate { get; set; }
    public string? ReturnReason { get; set; }
}