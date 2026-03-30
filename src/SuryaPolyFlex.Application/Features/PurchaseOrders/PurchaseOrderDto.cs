using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.PurchaseOrders;

public class PurchaseOrderDto
{
    public int Id { get; set; }
    public string PONumber { get; set; } = default!;
    public DateTime PODate { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; } = default!;
    public int? IndentId { get; set; }
    public string? IndentNumber { get; set; }
    public string Status { get; set; } = default!;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Terms { get; set; }
    public string? Remarks { get; set; }
    public decimal TotalAmount { get; set; }
    public List<POItemDto> Items { get; set; } = new();
}

public class POItemDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = default!;
    public string ItemName { get; set; } = default!;
    public string UoMCode { get; set; } = default!;
    public decimal OrderedQty { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal PendingQty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxPct { get; set; }
    public decimal LineTotal { get; set; }
    public string? Remarks { get; set; }
}

public class CreatePODto
{
    [Required] public int VendorId { get; set; }
    public int? IndentId { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Terms { get; set; }
    public string? Remarks { get; set; }

    [Required, MinLength(1, ErrorMessage = "Add at least one item.")]
    public List<CreatePOItemDto> Items { get; set; } = new();
}

public class CreatePOItemDto
{
    [Required] public int ItemId { get; set; }
    [Required, Range(0.001, double.MaxValue)] public decimal OrderedQty { get; set; }
    [Required, Range(0.01, double.MaxValue)]  public decimal UnitPrice { get; set; }
    public decimal TaxPct { get; set; } = 0;
    public string? Remarks { get; set; }
}
public class IndentItemForPODto
{
    public int ItemId { get; set; }
    public decimal Qty { get; set; } 
}